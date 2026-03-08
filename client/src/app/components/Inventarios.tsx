import { useState } from 'react';
import {
  productos as productosData,
  sucursales,
  movimientos as movimientosData,
} from '../data/mockData';
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
} from 'lucide-react';

export function Inventarios() {
  const [productos, setProductos] = useState(productosData);
  const [movimientos, setMovimientos] = useState(movimientosData);
  const [searchTerm, setSearchTerm] = useState('');
  const [showProductModal, setShowProductModal] = useState(false);
  const [showMovementModal, setShowMovementModal] = useState(false);
  const [editingProduct, setEditingProduct] = useState<any>(null);
  const [activeTab, setActiveTab] = useState<'productos' | 'movimientos'>('productos');

  const [productForm, setProductForm] = useState({
    nombre: '',
    descripcion: '',
    noParte: '',
    precio: 0,
    stock: 0,
    minimo: 0,
    sucursal: sucursales[0].nombre,
    categoria: 'Electrónica',
  });

  const [movementForm, setMovementForm] = useState({
    tipo: 'entrada' as 'entrada' | 'salida' | 'transferencia' | 'merma',
    producto: '',
    cantidad: 0,
    sucursal: sucursales[0].nombre,
    origen: '',
    destino: '',
  });

  const filteredProductos = productos.filter(
    (p) =>
      p.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
      p.noParte.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleProductSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (editingProduct) {
      setProductos(
        productos.map((p) =>
          p.id === editingProduct.id ? { ...p, ...productForm } : p
        )
      );
    } else {
      const newProduct = {
        id: String(productos.length + 1),
        ...productForm,
      };
      setProductos([...productos, newProduct]);
    }

    setShowProductModal(false);
    setEditingProduct(null);
    resetProductForm();
  };

  const resetProductForm = () => {
    setProductForm({
      nombre: '',
      descripcion: '',
      noParte: '',
      precio: 0,
      stock: 0,
      minimo: 0,
      sucursal: sucursales[0].nombre,
      categoria: 'Electrónica',
    });
  };

  const handleMovementSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const newMovement = {
      id: String(movimientos.length + 1),
      tipo: movementForm.tipo,
      producto: movementForm.producto,
      cantidad: movementForm.cantidad,
      fecha: new Date().toISOString().split('T')[0],
      usuario: 'Usuario Actual',
      sucursal: movementForm.sucursal,
      ...(movementForm.tipo === 'transferencia' && {
        origen: movementForm.origen,
        destino: movementForm.destino,
      }),
    };

    setMovimientos([newMovement, ...movimientos]);

    // Actualizar stock del producto
    setProductos(
      productos.map((p) => {
        if (p.nombre === movementForm.producto) {
          let newStock = p.stock;
          if (movementForm.tipo === 'entrada') {
            newStock += movementForm.cantidad;
          } else if (
            movementForm.tipo === 'salida' ||
            movementForm.tipo === 'merma'
          ) {
            newStock -= movementForm.cantidad;
          }
          return { ...p, stock: Math.max(0, newStock) };
        }
        return p;
      })
    );

    setShowMovementModal(false);
    resetMovementForm();
  };

  const resetMovementForm = () => {
    setMovementForm({
      tipo: 'entrada',
      producto: '',
      cantidad: 0,
      sucursal: sucursales[0].nombre,
      origen: '',
      destino: '',
    });
  };

  const handleEditProduct = (product: any) => {
    setEditingProduct(product);
    setProductForm(product);
    setShowProductModal(true);
  };

  const handleDeleteProduct = (productId: string) => {
    if (confirm('¿Estás seguro de que deseas eliminar este producto?')) {
      setProductos(productos.filter((p) => p.id !== productId));
    }
  };

  const alertasInventario = productos.filter((p) => p.stock < p.minimo);

  const movementIcons = {
    entrada: ArrowUpCircle,
    salida: ArrowDownCircle,
    transferencia: ArrowRightLeft,
    merma: XCircle,
  };

  const movementColors = {
    entrada: 'bg-green-50 text-green-700 border-green-200',
    salida: 'bg-blue-50 text-blue-700 border-blue-200',
    transferencia: 'bg-purple-50 text-purple-700 border-purple-200',
    merma: 'bg-red-50 text-red-700 border-red-200',
  };

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
              setEditingProduct(null);
              resetProductForm();
              setShowProductModal(true);
            }}
            className="flex items-center gap-2 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition"
          >
            <Plus className="w-5 h-5" />
            Nuevo Producto
          </button>
          <button
            onClick={() => {
              resetMovementForm();
              setShowMovementModal(true);
            }}
            className="flex items-center gap-2 px-4 py-2 bg-purple-500 hover:bg-purple-600 text-white rounded-lg transition"
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
            {alertasInventario.map((p) => (
              <span
                key={p.id}
                className="px-3 py-1 bg-white text-orange-700 rounded-full text-sm"
              >
                {p.nombre} ({p.stock}/{p.minimo})
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
                    <tr key={producto.id} className="hover:bg-gray-50">
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
                        {producto.noParte}
                      </td>
                      <td className="px-4 py-4 text-sm text-gray-900">
                        {producto.sucursal}
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
                          className="text-teal-600 hover:text-teal-900 mr-3"
                        >
                          <Edit2 className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => handleDeleteProduct(producto.id)}
                          className="text-red-600 hover:text-red-900"
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
                const Icon = movementIcons[movimiento.tipo];
                return (
                  <div
                    key={movimiento.id}
                    className={`p-4 rounded-lg border ${movementColors[movimiento.tipo]}`}
                  >
                    <div className="flex items-start justify-between">
                      <div className="flex items-start gap-3">
                        <Icon className="w-5 h-5 mt-0.5" />
                        <div>
                          <p className="font-medium">
                            {movimiento.tipo.charAt(0).toUpperCase() +
                              movimiento.tipo.slice(1)}
                          </p>
                          <p className="text-sm mt-1">
                            Producto: {movimiento.producto}
                          </p>
                          <p className="text-sm">
                            Cantidad: {movimiento.cantidad} unidades
                          </p>
                          {movimiento.tipo === 'transferencia' && (
                            <p className="text-sm">
                              {movimiento.origen} → {movimiento.destino}
                            </p>
                          )}
                        </div>
                      </div>
                      <div className="text-right text-sm">
                        <p>{movimiento.fecha}</p>
                        <p className="text-xs mt-1">{movimiento.usuario}</p>
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
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    No. Parte
                  </label>
                  <input
                    type="text"
                    value={productForm.noParte}
                    onChange={(e) =>
                      setProductForm({ ...productForm, noParte: e.target.value })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Categoría
                  </label>
                  <select
                    value={productForm.categoria}
                    onChange={(e) =>
                      setProductForm({ ...productForm, categoria: e.target.value })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  >
                    <option>Electrónica</option>
                    <option>Accesorios</option>
                    <option>Cables</option>
                    <option>Otros</option>
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Precio
                  </label>
                  <input
                    type="number"
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

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Stock Actual
                  </label>
                  <input
                    type="number"
                    value={productForm.stock}
                    onChange={(e) =>
                      setProductForm({
                        ...productForm,
                        stock: Number(e.target.value),
                      })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Stock Mínimo
                  </label>
                  <input
                    type="number"
                    value={productForm.minimo}
                    onChange={(e) =>
                      setProductForm({
                        ...productForm,
                        minimo: Number(e.target.value),
                      })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Sucursal
                  </label>
                  <select
                    value={productForm.sucursal}
                    onChange={(e) =>
                      setProductForm({ ...productForm, sucursal: e.target.value })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  >
                    {sucursales.map((s) => (
                      <option key={s.id} value={s.nombre}>
                        {s.nombre}
                      </option>
                    ))}
                  </select>
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
                  className="flex-1 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition"
                >
                  {editingProduct ? 'Actualizar' : 'Crear'}
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
                  value={movementForm.tipo}
                  onChange={(e) =>
                    setMovementForm({
                      ...movementForm,
                      tipo: e.target.value as any,
                    })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                >
                  <option value="entrada">Entrada</option>
                  <option value="salida">Salida</option>
                  <option value="transferencia">Transferencia</option>
                  <option value="merma">Merma</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Producto
                </label>
                <select
                  value={movementForm.producto}
                  onChange={(e) =>
                    setMovementForm({ ...movementForm, producto: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  required
                >
                  <option value="">Seleccionar producto</option>
                  {productos.map((p) => (
                    <option key={p.id} value={p.nombre}>
                      {p.nombre} - Stock: {p.stock}
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

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Sucursal
                </label>
                <select
                  value={movementForm.sucursal}
                  onChange={(e) =>
                    setMovementForm({ ...movementForm, sucursal: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                >
                  {sucursales.map((s) => (
                    <option key={s.id} value={s.nombre}>
                      {s.nombre}
                    </option>
                  ))}
                </select>
              </div>

              {movementForm.tipo === 'transferencia' && (
                <>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Origen
                    </label>
                    <select
                      value={movementForm.origen}
                      onChange={(e) =>
                        setMovementForm({ ...movementForm, origen: e.target.value })
                      }
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                      required
                    >
                      <option value="">Seleccionar origen</option>
                      {sucursales.map((s) => (
                        <option key={s.id} value={s.nombre}>
                          {s.nombre}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Destino
                    </label>
                    <select
                      value={movementForm.destino}
                      onChange={(e) =>
                        setMovementForm({
                          ...movementForm,
                          destino: e.target.value,
                        })
                      }
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                      required
                    >
                      <option value="">Seleccionar destino</option>
                      {sucursales.map((s) => (
                        <option key={s.id} value={s.nombre}>
                          {s.nombre}
                        </option>
                      ))}
                    </select>
                  </div>
                </>
              )}

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
                  className="flex-1 px-4 py-2 bg-purple-500 hover:bg-purple-600 text-white rounded-lg transition"
                >
                  Registrar
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
