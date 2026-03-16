import { useState, useEffect } from 'react';
import {
  inventarioApi,
  productosApi,
  movimientosApi,
  sucursalesApi,
  tiposMovimientoApi,
  usuariosApi,
} from '../services/api';
import type {
  InventarioDto,
  ProductoDto,
  MovimientoInventarioDto,
  SucursalDto,
  TipoMovimientoDto,
} from '../types';
import {
  Package,
  Plus,
  Search,
  Edit2,
  Trash2,
  AlertTriangle,
  ArrowUpCircle,
  ArrowDownCircle,
  ArrowRightLeft,
  XCircle,
  Loader2,
} from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { toast } from 'sonner';

export function Inventarios() {
  const { user } = useAuth();
  const [inventario, setInventario] = useState<InventarioDto[]>([]);
  const [productos, setProductos] = useState<ProductoDto[]>([]);
  const [movimientos, setMovimientos] = useState<MovimientoInventarioDto[]>([]);
  const [sucursales, setSucursales] = useState<SucursalDto[]>([]);
  const [sucursalesPermitidas, setSucursalesPermitidas] = useState<SucursalDto[]>([]);
  const [tiposMovimiento, setTiposMovimiento] = useState<TipoMovimientoDto[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [showProductModal, setShowProductModal] = useState(false);
  const [showMovementModal, setShowMovementModal] = useState(false);
  const [editingProduct, setEditingProduct] = useState<ProductoDto | null>(null);
  const [activeTab, setActiveTab] = useState<'productos' | 'movimientos'>('productos');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const [productForm, setProductForm] = useState({
    nombre: '',
    descripcion: '',
    numeroParte: '',
    precio: 0,
  });

  const [movementForm, setMovementForm] = useState({
    idTipoMovimiento: 0,
    idProducto: 0,
    cantidad: 0,
    idSucursalOrigen: 0,
    idSucursalDestino: 0,
    observaciones: '',
  });

  const normalize = (value?: string) => (value ?? '').trim().toLowerCase();

  const hasPermiso = (
    modulo: string,
    categoria: string,
    accion: 'leer' | 'escribir' | 'eliminar'
  ) => {
    const moduloNorm = normalize(modulo);
    const categoriaNorm = normalize(categoria);

    return (user?.permisos ?? []).some((p) => {
      const sameModulo = normalize(p.nombreModulo) === moduloNorm;
      const sameCategoria = normalize(p.nombreCategoria) === categoriaNorm;

      if (!sameModulo || !sameCategoria) return false;

      if (accion === 'leer') return p.puedeLeer;
      if (accion === 'escribir') return p.puedeEscribir;
      return p.puedeEliminar;
    });
  };

  const canWriteProductos = hasPermiso('Inventario', 'Productos', 'escribir');
  const canDeleteProductos = hasPermiso('Inventario', 'Productos', 'eliminar');
  const canWriteMovimientos = hasPermiso('Inventario', 'Movimientos', 'escribir');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [inv, prods, movs, sucs, tipos] = await Promise.all([
        inventarioApi.getAll(),
        productosApi.getAll(),
        movimientosApi.getAll(),
        sucursalesApi.getAll(),
        tiposMovimientoApi.getAll(),
      ]);

      let sucursalesHabilitadas = sucs;
      if (user?.idUsuario) {
        const usuarioDetalle = await usuariosApi.getById(user.idUsuario);
        const idsPermitidos = new Set(usuarioDetalle.sucursales.map((s) => s.idSucursal));
        sucursalesHabilitadas = sucs.filter((s) => idsPermitidos.has(s.idSucursal));
      }

      setInventario(inv);
      setProductos(prods);
      setMovimientos(movs);
      setSucursales(sucs);
      setSucursalesPermitidas(sucursalesHabilitadas);
      setTiposMovimiento(tipos);
    } catch (err) {
      console.error('Error al cargar datos:', err);
    } finally {
      setLoading(false);
    }
  };

  // Combinar productos con datos de inventario
  const productosConStock = productos.map((p) => {
    const invItems = inventario.filter((i) => i.idProducto === p.idProducto);
    const stockTotal = invItems.reduce((sum, i) => sum + i.stockActual, 0);
    const stockMinimo = invItems.length > 0 ? Math.min(...invItems.map((i) => i.stockMinimo)) : 0;
    return { ...p, stock: stockTotal, minimo: stockMinimo, invItems };
  });

  const filteredProductos = productosConStock.filter(
    (p) =>
      p.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
      p.numeroParte.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const alertasInventario = inventario.filter((i) => i.stockBajo);

  const handleProductSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!canWriteProductos) {
      toast.error('No tienes permisos para crear o editar productos.');
      return;
    }

    setSaving(true);

    try {
      if (editingProduct) {
        await productosApi.update(editingProduct.idProducto, productForm);
      } else {
        await productosApi.create(productForm);
      }

      setShowProductModal(false);
      setEditingProduct(null);
      resetProductForm();
      await loadData();
      toast.success(editingProduct ? 'Producto actualizado correctamente.' : 'Producto creado correctamente.');
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Error al guardar producto');
    } finally {
      setSaving(false);
    }
  };

  const resetProductForm = () => {
    setProductForm({
      nombre: '',
      descripcion: '',
      numeroParte: '',
      precio: 0,
    });
  };

  const handleMovementSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!canWriteMovimientos) {
      toast.error('No tienes permisos para registrar movimientos.');
      return;
    }

    if (!movementValidation.canSubmit) {
      toast.error(movementValidation.message ?? 'Completa los datos para registrar el movimiento.');
      return;
    }

    if (!user?.idUsuario) {
      toast.error('No hay un usuario autenticado para registrar el movimiento.');
      return;
    }

    const kind = selectedMovementKind;

    setSaving(true);

    try {
      await movimientosApi.registrar({
        idProducto: movementForm.idProducto,
        idTipoMovimiento: movementForm.idTipoMovimiento,
        cantidad: movementForm.cantidad,
        idUsuario: user.idUsuario,
        idSucursalOrigen:
          kind === 'salida' || kind === 'transferencia'
            ? movementForm.idSucursalOrigen || undefined
            : undefined,
        idSucursalDestino:
          kind === 'entrada' || kind === 'transferencia'
            ? movementForm.idSucursalDestino || undefined
            : undefined,
        observaciones: movementForm.observaciones || undefined,
      });

      setShowMovementModal(false);
      resetMovementForm();
      await loadData();
      toast.success('Movimiento registrado correctamente.');
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Error al registrar movimiento');
    } finally {
      setSaving(false);
    }
  };

  const resetMovementForm = () => {
    setMovementForm({
      idTipoMovimiento: tiposMovimiento[0]?.idTipoMovimiento ?? 0,
      idProducto: 0,
      cantidad: 0,
      idSucursalOrigen: 0,
      idSucursalDestino: 0,
      observaciones: '',
    });
  };

  const handleEditProduct = (product: ProductoDto) => {
    if (!canWriteProductos) {
      toast.error('No tienes permisos para editar productos.');
      return;
    }

    setEditingProduct(product);
    setProductForm({
      nombre: product.nombre,
      descripcion: product.descripcion ?? '',
      numeroParte: product.numeroParte,
      precio: product.precio,
    });
    setShowProductModal(true);
  };

  const handleDeleteProduct = async (productId: number) => {
    if (!canDeleteProductos) {
      toast.error('No tienes permisos para eliminar productos.');
      return;
    }

    if (confirm('¿Estás seguro de que deseas desactivar este producto?')) {
      try {
        await productosApi.desactivar(productId);
        await loadData();
        toast.success('Producto desactivado correctamente.');
      } catch (err) {
        toast.error(err instanceof Error ? err.message : 'Error al eliminar');
      }
    }
  };

  const getMovementIcon = (tipo: string) => {
    const t = tipo.toLowerCase();
    if (t.includes('compra') || t.includes('entrada')) return ArrowUpCircle;
    if (t.includes('venta') || t.includes('salida')) return ArrowDownCircle;
    if (t.includes('transferencia')) return ArrowRightLeft;
    if (t.includes('merma')) return XCircle;
    return ArrowUpCircle;
  };

  const getMovementColor = (tipo: string) => {
    const t = tipo.toLowerCase();
    if (t.includes('compra') || t.includes('entrada')) return 'bg-green-50 text-green-700 border-green-200';
    if (t.includes('venta') || t.includes('salida')) return 'bg-blue-50 text-blue-700 border-blue-200';
    if (t.includes('transferencia')) return 'bg-purple-50 text-purple-700 border-purple-200';
    if (t.includes('merma')) return 'bg-red-50 text-red-700 border-red-200';
    return 'bg-gray-50 text-gray-700 border-gray-200';
  };

  const inferMovementKind = (tipo?: TipoMovimientoDto): 'entrada' | 'salida' | 'transferencia' => {
    if (!tipo) return 'salida';
    if (tipo.esTransferencia) return 'transferencia';

    const nombre = tipo.nombre.toLowerCase();
    if (nombre.includes('compra') || nombre.includes('entrada')) return 'entrada';

    return 'salida';
  };

  const selectedTipo = tiposMovimiento.find((t) => t.idTipoMovimiento === movementForm.idTipoMovimiento);
  const selectedMovementKind = inferMovementKind(selectedTipo);
  const sucursalReferenciaId =
    selectedMovementKind === 'entrada'
      ? movementForm.idSucursalDestino
      : movementForm.idSucursalOrigen;

  const inventarioSucursalProducto = inventario.find(
    (i) => i.idProducto === movementForm.idProducto && i.idSucursal === sucursalReferenciaId
  );

  const productosDisponiblesSegunSucursal =
    selectedMovementKind === 'salida' || selectedMovementKind === 'transferencia'
      ? sucursalReferenciaId > 0
        ? productos.filter((p) =>
            inventario.some((i) => i.idSucursal === sucursalReferenciaId && i.idProducto === p.idProducto)
          )
        : []
      : productos;

  const movementValidation: { canSubmit: boolean; message?: string } = (() => {
    if (!user?.idUsuario) return { canSubmit: false, message: 'No hay un usuario autenticado.' };
    if (sucursalesPermitidas.length === 0) return { canSubmit: false, message: 'No tienes sucursales asignadas.' };
    if (!selectedTipo) return { canSubmit: false, message: 'Debes seleccionar un tipo de movimiento.' };

    if (selectedMovementKind === 'entrada' && movementForm.idSucursalDestino === 0) {
      return { canSubmit: false, message: 'Debes seleccionar una sucursal destino.' };
    }

    if ((selectedMovementKind === 'salida' || selectedMovementKind === 'transferencia') && movementForm.idSucursalOrigen === 0) {
      return { canSubmit: false, message: 'Debes seleccionar una sucursal origen.' };
    }

    if (selectedMovementKind === 'transferencia' && movementForm.idSucursalDestino === 0) {
      return { canSubmit: false, message: 'Debes seleccionar una sucursal destino.' };
    }

    if (
      selectedMovementKind === 'transferencia' &&
      movementForm.idSucursalOrigen > 0 &&
      movementForm.idSucursalDestino > 0 &&
      movementForm.idSucursalOrigen === movementForm.idSucursalDestino
    ) {
      return { canSubmit: false, message: 'La sucursal origen y destino no pueden ser la misma.' };
    }

    if (movementForm.idProducto === 0) return { canSubmit: false, message: 'Debes seleccionar un producto.' };
    if (movementForm.cantidad <= 0) return { canSubmit: false, message: 'La cantidad debe ser mayor a 0.' };

    if (selectedMovementKind === 'salida' || selectedMovementKind === 'transferencia') {
      if (!inventarioSucursalProducto) {
        return { canSubmit: false, message: 'No existe inventario del producto en la sucursal origen seleccionada.' };
      }

      if (inventarioSucursalProducto.stockActual < movementForm.cantidad) {
        return {
          canSubmit: false,
          message: `Stock insuficiente en origen. Disponible: ${inventarioSucursalProducto.stockActual}.`,
        };
      }
    }

    return { canSubmit: true };
  })();

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="w-8 h-8 animate-spin text-teal-500" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Gestión de Inventarios</h1>
          <p className="text-gray-600 mt-1">Control de productos y stock</p>
        </div>
        <div className="flex gap-2">
          <button
            onClick={() => {
              if (!canWriteProductos) {
                toast.error('No tienes permisos para crear productos.');
                return;
              }

              setEditingProduct(null);
              resetProductForm();
              setShowProductModal(true);
            }}
            disabled={!canWriteProductos}
            className="flex items-center gap-2 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition disabled:opacity-50"
          >
            <Plus className="w-5 h-5" />
            Nuevo Producto
          </button>
          <button
            onClick={() => {
              if (!canWriteMovimientos) {
                toast.error('No tienes permisos para registrar movimientos.');
                return;
              }

              resetMovementForm();
              setShowMovementModal(true);
            }}
            disabled={!canWriteMovimientos}
            className="flex items-center gap-2 px-4 py-2 bg-purple-500 hover:bg-purple-600 text-white rounded-lg transition disabled:opacity-50"
          >
            <ArrowRightLeft className="w-5 h-5" />
            Registrar Movimiento
          </button>
        </div>
      </div>

      {/* Alertas */}
      {alertasInventario.length > 0 && (
        <div className="bg-orange-50 border border-orange-200 rounded-xl p-4">
          <div className="flex items-center gap-2 mb-2">
            <AlertTriangle className="w-5 h-5 text-orange-600" />
            <h3 className="font-semibold text-orange-900">
              {alertasInventario.length} producto(s) con stock bajo
            </h3>
          </div>
          <div className="flex flex-wrap gap-2">
            {alertasInventario.map((a) => (
              <span
                key={a.idInventario}
                className="px-3 py-1 bg-white text-orange-700 rounded-full text-sm"
              >
                {a.nombreProducto} ({a.stockActual}/{a.stockMinimo})
              </span>
            ))}
          </div>
        </div>
      )}

      {/* Tabs */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100">
        <div className="border-b border-gray-200">
          <nav className="flex">
            <button
              onClick={() => setActiveTab('productos')}
              className={`px-6 py-3 font-medium transition ${
                activeTab === 'productos'
                  ? 'text-teal-600 border-b-2 border-teal-600'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              Productos
            </button>
            <button
              onClick={() => setActiveTab('movimientos')}
              className={`px-6 py-3 font-medium transition ${
                activeTab === 'movimientos'
                  ? 'text-teal-600 border-b-2 border-teal-600'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              Movimientos
            </button>
          </nav>
        </div>

        <div className="p-4">
          {/* Búsqueda (solo para productos) */}
          {activeTab === 'productos' && (
            <div className="mb-4">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                <input
                  type="text"
                  placeholder="Buscar por nombre o número de parte..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                />
              </div>
            </div>
          )}

          {/* Contenido de productos */}
          {activeTab === 'productos' && (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50 border-b border-gray-200">
                  <tr>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                      Producto
                    </th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                      No. Parte
                    </th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                      Sucursal
                    </th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                      Stock
                    </th>
                    <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                      Precio
                    </th>
                    <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                      Acciones
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {filteredProductos.map((producto) => (
                    <tr key={producto.idProducto} className="hover:bg-gray-50">
                      <td className="px-4 py-4">
                        <div>
                          <p className="font-medium text-gray-900">
                            {producto.nombre}
                          </p>
                          <p className="text-sm text-gray-500">
                            {producto.descripcion}
                          </p>
                        </div>
                      </td>
                      <td className="px-4 py-4 text-sm text-gray-900">
                        {producto.numeroParte}
                      </td>
                      <td className="px-4 py-4 text-sm text-gray-900">
                        {producto.invItems.length > 0
                          ? producto.invItems.map((i) => i.nombreSucursal).join(', ')
                          : 'Sin asignar'}
                      </td>
                      <td className="px-4 py-4">
                        <span
                          className={`px-2 py-1 text-xs font-medium rounded-full ${
                            producto.stock < producto.minimo
                              ? 'bg-red-100 text-red-700'
                              : 'bg-green-100 text-green-700'
                          }`}
                        >
                          {producto.stock} / {producto.minimo}
                        </span>
                      </td>
                      <td className="px-4 py-4 text-sm text-gray-900">
                        ${producto.precio.toLocaleString()}
                      </td>
                      <td className="px-4 py-4 text-right">
                        <button
                          onClick={() => handleEditProduct(producto)}
                          disabled={!canWriteProductos}
                          className="text-teal-600 hover:text-teal-900 mr-3 disabled:opacity-40"
                        >
                          <Edit2 className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => handleDeleteProduct(producto.idProducto)}
                          disabled={!canDeleteProductos}
                          className="text-red-600 hover:text-red-900 disabled:opacity-40"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {/* Contenido de movimientos */}
          {activeTab === 'movimientos' && (
            <div className="space-y-3">
              {movimientos.map((movimiento) => {
                const tipoNombre = movimiento.nombreTipoMovimiento ?? 'Movimiento';
                const Icon = getMovementIcon(tipoNombre);
                return (
                  <div
                    key={movimiento.idMovimiento}
                    className={`p-4 rounded-lg border ${getMovementColor(tipoNombre)}`}
                  >
                    <div className="flex items-start justify-between">
                      <div className="flex items-start gap-3">
                        <Icon className="w-5 h-5 mt-0.5" />
                        <div>
                          <p className="font-medium">
                            {movimiento.nombreTipoMovimiento}
                          </p>
                          <p className="text-sm mt-1">
                            Producto: {movimiento.nombreProducto}
                          </p>
                          <p className="text-sm">
                            Cantidad: {movimiento.cantidad} unidades
                          </p>
                          {movimiento.observaciones && (
                            <p className="text-sm">
                              Obs: {movimiento.observaciones}
                            </p>
                          )}
                        </div>
                      </div>
                      <div className="text-right text-sm">
                        <p>{new Date(movimiento.fecha).toLocaleDateString()}</p>
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>

      {/* Modal de producto */}
      {showProductModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl max-w-lg w-full p-6 max-h-[90vh] overflow-y-auto">
            <h2 className="text-xl font-bold text-gray-900 mb-4">
              {editingProduct ? 'Editar Producto' : 'Nuevo Producto'}
            </h2>
            <form onSubmit={handleProductSubmit} className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="col-span-2">
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Nombre
                  </label>
                  <input
                    type="text"
                    value={productForm.nombre}
                    onChange={(e) =>
                      setProductForm({ ...productForm, nombre: e.target.value })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                    required
                  />
                </div>

                <div className="col-span-2">
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Descripción
                  </label>
                  <textarea
                    value={productForm.descripcion}
                    onChange={(e) =>
                      setProductForm({
                        ...productForm,
                        descripcion: e.target.value,
                      })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                    rows={2}
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    No. Parte
                  </label>
                  <input
                    type="text"
                    value={productForm.numeroParte}
                    onChange={(e) =>
                      setProductForm({ ...productForm, numeroParte: e.target.value })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Precio
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    value={productForm.precio}
                    onChange={(e) =>
                      setProductForm({
                        ...productForm,
                        precio: Number(e.target.value),
                      })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                    required
                  />
                </div>
              </div>

              <div className="flex gap-3 pt-4">
                <button
                  type="button"
                  onClick={() => {
                    setShowProductModal(false);
                    setEditingProduct(null);
                  }}
                  className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={saving || !canWriteProductos}
                  className="flex-1 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition disabled:opacity-50"
                >
                  {saving ? 'Guardando...' : editingProduct ? 'Actualizar' : 'Crear'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Modal de movimiento */}
      {showMovementModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl max-w-md w-full p-6">
            <h2 className="text-xl font-bold text-gray-900 mb-4">
              Registrar Movimiento
            </h2>
            <form onSubmit={handleMovementSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Tipo de Movimiento
                </label>
                <select
                  value={movementForm.idTipoMovimiento}
                  onChange={(e) =>
                    setMovementForm({
                      ...movementForm,
                      idTipoMovimiento: Number(e.target.value),
                      idSucursalOrigen: 0,
                      idSucursalDestino: 0,
                      idProducto: 0,
                    })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  required
                >
                  <option value={0}>Seleccionar tipo</option>
                  {tiposMovimiento.map((t) => (
                    <option key={t.idTipoMovimiento} value={t.idTipoMovimiento}>
                      {t.nombre}
                    </option>
                  ))}
                </select>
              </div>

              {(selectedMovementKind === 'salida' || selectedMovementKind === 'transferencia') && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Sucursal Origen
                  </label>
                  <select
                    value={movementForm.idSucursalOrigen}
                    onChange={(e) =>
                      setMovementForm({
                        ...movementForm,
                        idSucursalOrigen: Number(e.target.value),
                        idProducto: 0,
                      })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  >
                    <option value={0}>Seleccionar origen</option>
                    {sucursalesPermitidas.map((s) => (
                      <option key={s.idSucursal} value={s.idSucursal}>
                        {s.nombre}
                      </option>
                    ))}
                  </select>
                </div>
              )}

              {(selectedMovementKind === 'entrada' || selectedMovementKind === 'transferencia') && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Sucursal Destino
                  </label>
                  <select
                    value={movementForm.idSucursalDestino}
                    onChange={(e) =>
                      setMovementForm({
                        ...movementForm,
                        idSucursalDestino: Number(e.target.value),
                        idProducto: 0,
                      })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  >
                    <option value={0}>Seleccionar destino</option>
                    {sucursalesPermitidas
                      .filter((s) =>
                        selectedMovementKind === 'transferencia'
                          ? s.idSucursal !== movementForm.idSucursalOrigen
                          : true
                      )
                      .map((s) => (
                        <option key={s.idSucursal} value={s.idSucursal}>
                          {s.nombre}
                        </option>
                      ))}
                  </select>
                </div>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Producto
                </label>
                <select
                  value={movementForm.idProducto}
                  onChange={(e) =>
                    setMovementForm({ ...movementForm, idProducto: Number(e.target.value) })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  required
                  disabled={sucursalReferenciaId === 0}
                >
                  <option value={0}>
                    {sucursalReferenciaId === 0
                      ? 'Primero selecciona una sucursal'
                      : 'Seleccionar producto'}
                  </option>
                  {productosDisponiblesSegunSucursal.map((p) => (
                    <option key={p.idProducto} value={p.idProducto}>
                      {p.nombre} - {p.numeroParte}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Cantidad
                </label>
                <input
                  type="number"
                  value={movementForm.cantidad}
                  onChange={(e) =>
                    setMovementForm({
                      ...movementForm,
                      cantidad: Number(e.target.value),
                    })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  min="1"
                  required
                />
              </div>

              {sucursalReferenciaId > 0 &&
                movementForm.idProducto > 0 &&
                (
                  <p
                    className={`text-sm rounded-lg p-2 border ${
                      inventarioSucursalProducto
                        ? 'text-gray-700 bg-gray-50 border-gray-200'
                        : 'text-red-700 bg-red-50 border-red-200'
                    }`}
                  >
                    {inventarioSucursalProducto
                      ? `Existencia actual en la sucursal seleccionada: ${inventarioSucursalProducto.stockActual}`
                      : 'No existe inventario de este producto en la sucursal seleccionada.'}
                  </p>
                )}

              {sucursalesPermitidas.length === 0 && (
                <p className="text-sm text-amber-700 bg-amber-50 border border-amber-200 rounded-lg p-2">
                  No tienes sucursales asignadas. Solicita a un administrador que te asigne al menos una sucursal.
                </p>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Observaciones
                </label>
                <textarea
                  value={movementForm.observaciones}
                  onChange={(e) =>
                    setMovementForm({ ...movementForm, observaciones: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  rows={2}
                />
              </div>

              <div className="flex gap-3 pt-4">
                <button
                  type="button"
                  onClick={() => setShowMovementModal(false)}
                  className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={saving || !movementValidation.canSubmit || !canWriteMovimientos}
                  className="flex-1 px-4 py-2 bg-purple-500 hover:bg-purple-600 text-white rounded-lg transition disabled:opacity-50"
                >
                  {saving ? 'Registrando...' : 'Registrar'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
