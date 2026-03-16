import { useState, useEffect } from 'react';
import { UserPlus, Search, Edit2, Trash2, UserCheck, UserX, Loader2, Building2, Plus, X } from 'lucide-react';
import { usuariosApi, rolesApi, sucursalesApi } from '../services/api';
import type { UsuarioDto, RolDto, UsuarioDetalleDto, SucursalDto } from '../types';
import { useAuth } from '../context/AuthContext';
import { hasPermission } from '../utils/permissions';

export function Usuarios() {
  const { user } = useAuth();
  const [usuarios, setUsuarios] = useState<UsuarioDto[]>([]);
  const [roles, setRoles] = useState<RolDto[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [showSucursalesModal, setShowSucursalesModal] = useState(false);
  const [editingUser, setEditingUser] = useState<UsuarioDto | null>(null);
  const [selectedUserDetalle, setSelectedUserDetalle] = useState<UsuarioDetalleDto | null>(null);
  const [allSucursales, setAllSucursales] = useState<SucursalDto[]>([]);
  const [idSucursalToAssign, setIdSucursalToAssign] = useState(0);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [loadingSucursales, setLoadingSucursales] = useState(false);

  const [formData, setFormData] = useState({
    nombre: '',
    correo: '',
    contrasenia: '',
    idRol: 0,
  });

  const canWriteUsuarios = hasPermission(user, {
    modulo: 'Seguridad',
    categoria: 'Usuarios',
    accion: 'escribir',
  });

  const canDeleteUsuarios = hasPermission(user, {
    modulo: 'Seguridad',
    categoria: 'Usuarios',
    accion: 'eliminar',
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [usrs, rls] = await Promise.all([
        usuariosApi.getAll(),
        rolesApi.getAll(),
      ]);
      setUsuarios(usrs);
      setRoles(rls);
    } catch (err) {
      console.error('Error al cargar datos:', err);
    } finally {
      setLoading(false);
    }
  };

  const filteredUsuarios = usuarios.filter(
    (user) =>
      user.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.correo.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!canWriteUsuarios) {
      alert('No tienes permisos para crear o editar usuarios.');
      return;
    }

    setSaving(true);

    try {
      if (editingUser) {
        await usuariosApi.update(editingUser.idUsuario, {
          nombre: formData.nombre,
          correo: formData.correo,
          idRol: formData.idRol,
        });
      } else {
        await usuariosApi.create({
          nombre: formData.nombre,
          correo: formData.correo,
          contrasenia: formData.contrasenia,
          idRol: formData.idRol,
        });
      }

      setShowModal(false);
      setEditingUser(null);
      await loadData();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Error al guardar');
    } finally {
      setSaving(false);
    }
  };

  const handleEdit = (user: UsuarioDto) => {
    if (!canWriteUsuarios) {
      alert('No tienes permisos para editar usuarios.');
      return;
    }

    setEditingUser(user);
    setFormData({
      nombre: user.nombre,
      correo: user.correo,
      contrasenia: '',
      idRol: user.idRol,
    });
    setShowModal(true);
  };

  const handleDelete = async (userId: number) => {
    if (!canDeleteUsuarios) {
      alert('No tienes permisos para desactivar usuarios.');
      return;
    }

    if (confirm('¿Estás seguro de que deseas desactivar este usuario?')) {
      try {
        await usuariosApi.desactivar(userId);
        await loadData();
      } catch (err) {
        alert(err instanceof Error ? err.message : 'Error al eliminar');
      }
    }
  };

  const openSucursalesModal = async (userId: number) => {
    if (!canWriteUsuarios) {
      alert('No tienes permisos para asignar sucursales a usuarios.');
      return;
    }

    try {
      setLoadingSucursales(true);
      const [userDetalle, sucursales] = await Promise.all([
        usuariosApi.getById(userId),
        sucursalesApi.getAll(),
      ]);

      setSelectedUserDetalle(userDetalle);
      setAllSucursales(sucursales.filter((s) => s.activa));
      setIdSucursalToAssign(0);
      setShowSucursalesModal(true);
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Error al cargar sucursales del usuario');
    } finally {
      setLoadingSucursales(false);
    }
  };

  const refreshSelectedUserDetalle = async () => {
    if (!selectedUserDetalle) return;
    const updated = await usuariosApi.getById(selectedUserDetalle.idUsuario);
    setSelectedUserDetalle(updated);
  };

  const handleAsignarSucursal = async () => {
    if (!canWriteUsuarios) {
      alert('No tienes permisos para asignar sucursales a usuarios.');
      return;
    }

    if (!selectedUserDetalle || idSucursalToAssign === 0) return;

    setSaving(true);
    try {
      await usuariosApi.asignarSucursal(selectedUserDetalle.idUsuario, {
        idSucursal: idSucursalToAssign,
      });
      await refreshSelectedUserDetalle();
      setIdSucursalToAssign(0);
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Error al asignar sucursal');
    } finally {
      setSaving(false);
    }
  };

  const handleRemoverSucursal = async (idSucursal: number) => {
    if (!canWriteUsuarios) {
      alert('No tienes permisos para remover sucursales de usuarios.');
      return;
    }

    if (!selectedUserDetalle) return;

    setSaving(true);
    try {
      await usuariosApi.removerSucursal(selectedUserDetalle.idUsuario, idSucursal);
      await refreshSelectedUserDetalle();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Error al remover sucursal');
    } finally {
      setSaving(false);
    }
  };

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
          <h1 className="text-3xl font-bold text-gray-900">Gestión de Usuarios</h1>
          <p className="text-gray-600 mt-1">
            Administra los usuarios del sistema
          </p>
        </div>
        <button
          onClick={() => {
            if (!canWriteUsuarios) {
              alert('No tienes permisos para crear usuarios.');
              return;
            }

            setEditingUser(null);
            setFormData({
              nombre: '',
              correo: '',
              contrasenia: '',
              idRol: roles[0]?.idRol ?? 0,
            });
            setShowModal(true);
          }}
          disabled={!canWriteUsuarios}
          className="flex items-center gap-2 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition disabled:opacity-50"
        >
          <UserPlus className="w-5 h-5" />
          Nuevo Usuario
        </button>
      </div>

      {/* Búsqueda */}
      <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
          <input
            type="text"
            placeholder="Buscar por nombre o correo..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
          />
        </div>
      </div>

      {/* Tabla de usuarios */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Usuario
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Rol
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Estado
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Acciones
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {filteredUsuarios.map((usuario) => (
                <tr key={usuario.idUsuario} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="flex items-center">
                      <div className="w-10 h-10 bg-teal-100 rounded-full flex items-center justify-center flex-shrink-0">
                        <span className="text-teal-700 font-medium">
                          {usuario.nombre.charAt(0)}
                        </span>
                      </div>
                      <div className="ml-4">
                        <div className="font-medium text-gray-900">
                          {usuario.nombre}
                        </div>
                        <div className="text-sm text-gray-500">{usuario.correo}</div>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className="px-2 py-1 text-xs font-medium bg-purple-100 text-purple-700 rounded-full">
                      {usuario.nombreRol ?? 'Sin rol'}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span
                      className={`flex items-center gap-1 px-2 py-1 text-xs font-medium rounded-full w-fit ${
                        usuario.activo
                          ? 'bg-green-100 text-green-700'
                          : 'bg-red-100 text-red-700'
                      }`}
                    >
                      {usuario.activo ? (
                        <>
                          <UserCheck className="w-3 h-3" />
                          Activo
                        </>
                      ) : (
                        <>
                          <UserX className="w-3 h-3" />
                          Inactivo
                        </>
                      )}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => openSucursalesModal(usuario.idUsuario)}
                      disabled={!canWriteUsuarios}
                      className="text-indigo-600 hover:text-indigo-900 mr-3 disabled:opacity-40"
                      title="Asignar sucursales"
                    >
                      <Building2 className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => handleEdit(usuario)}
                      disabled={!canWriteUsuarios}
                      className="text-teal-600 hover:text-teal-900 mr-3 disabled:opacity-40"
                    >
                      <Edit2 className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => handleDelete(usuario.idUsuario)}
                      disabled={!canDeleteUsuarios}
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
      </div>

      {/* Modal de creación/edición */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl max-w-md w-full p-6">
            <h2 className="text-xl font-bold text-gray-900 mb-4">
              {editingUser ? 'Editar Usuario' : 'Nuevo Usuario'}
            </h2>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Nombre Completo
                </label>
                <input
                  type="text"
                  value={formData.nombre}
                  onChange={(e) =>
                    setFormData({ ...formData, nombre: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Correo Electrónico
                </label>
                <input
                  type="email"
                  value={formData.correo}
                  onChange={(e) =>
                    setFormData({ ...formData, correo: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  required
                />
              </div>

              {!editingUser && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Contraseña
                  </label>
                  <input
                    type="password"
                    value={formData.contrasenia}
                    onChange={(e) =>
                      setFormData({ ...formData, contrasenia: e.target.value })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                    required
                    minLength={6}
                  />
                </div>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Rol
                </label>
                <select
                  value={formData.idRol}
                  onChange={(e) =>
                    setFormData({ ...formData, idRol: Number(e.target.value) })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                >
                  {roles.map((rol) => (
                    <option key={rol.idRol} value={rol.idRol}>
                      {rol.nombre}
                    </option>
                  ))}
                </select>
              </div>

              <div className="flex gap-3 pt-4">
                <button
                  type="button"
                  onClick={() => {
                    setShowModal(false);
                    setEditingUser(null);
                  }}
                  className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={saving || !canWriteUsuarios}
                  className="flex-1 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition disabled:opacity-50"
                >
                  {saving ? 'Guardando...' : editingUser ? 'Actualizar' : 'Crear'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Modal de asignación de sucursales */}
      {showSucursalesModal && selectedUserDetalle && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl max-w-lg w-full p-6">
            <h2 className="text-xl font-bold text-gray-900 mb-2">
              Sucursales de {selectedUserDetalle.nombre}
            </h2>
            <p className="text-sm text-gray-600 mb-4">Solo en estas sucursales podrá registrar movimientos.</p>

            <div className="flex gap-2 mb-4">
              <select
                value={idSucursalToAssign}
                onChange={(e) => setIdSucursalToAssign(Number(e.target.value))}
                className="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                disabled={saving || loadingSucursales}
              >
                <option value={0}>Seleccionar sucursal para asignar</option>
                {allSucursales
                  .filter(
                    (s) => !selectedUserDetalle.sucursales.some((us) => us.idSucursal === s.idSucursal)
                  )
                  .map((sucursal) => (
                    <option key={sucursal.idSucursal} value={sucursal.idSucursal}>
                      {sucursal.nombre}
                    </option>
                  ))}
              </select>
              <button
                type="button"
                onClick={handleAsignarSucursal}
                disabled={saving || idSucursalToAssign === 0 || !canWriteUsuarios}
                className="px-3 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg disabled:opacity-50"
                title="Asignar"
              >
                <Plus className="w-4 h-4" />
              </button>
            </div>

            <div className="space-y-2 max-h-64 overflow-y-auto mb-4">
              {selectedUserDetalle.sucursales.length === 0 && (
                <div className="text-sm text-gray-500 bg-gray-50 border border-gray-200 rounded-lg p-3">
                  Este usuario no tiene sucursales asignadas.
                </div>
              )}

              {selectedUserDetalle.sucursales.map((sucursal) => (
                <div
                  key={sucursal.idSucursal}
                  className="flex items-center justify-between px-3 py-2 border border-gray-200 rounded-lg"
                >
                  <div>
                    <p className="font-medium text-gray-900">{sucursal.nombre}</p>
                    <p className="text-xs text-gray-500">{sucursal.ciudad}, {sucursal.estado}</p>
                  </div>
                  <button
                    type="button"
                    onClick={() => handleRemoverSucursal(sucursal.idSucursal)}
                    disabled={saving || !canWriteUsuarios}
                    className="text-red-600 hover:text-red-900 disabled:opacity-50"
                    title="Remover asignación"
                  >
                    <X className="w-4 h-4" />
                  </button>
                </div>
              ))}
            </div>

            <div className="flex justify-end">
              <button
                type="button"
                onClick={() => {
                  setShowSucursalesModal(false);
                  setSelectedUserDetalle(null);
                  setIdSucursalToAssign(0);
                }}
                className="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition"
              >
                Cerrar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
