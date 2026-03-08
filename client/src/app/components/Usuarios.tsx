import { useState } from 'react';
import { usuarios as usuariosData, perfiles, sucursales } from '../data/mockData';
import { UserPlus, Search, Edit2, Trash2, UserCheck, UserX } from 'lucide-react';

export function Usuarios() {
  const [usuarios, setUsuarios] = useState(usuariosData);
  const [searchTerm, setSearchTerm] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingUser, setEditingUser] = useState<any>(null);

  const [formData, setFormData] = useState({
    nombre: '',
    email: '',
    password: '',
    rol: perfiles[0].nombre,
    sucursal: sucursales[0].nombre,
  });

  const filteredUsuarios = usuarios.filter(
    (user) =>
      user.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.email.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (editingUser) {
      // Actualizar usuario existente
      setUsuarios(
        usuarios.map((u) =>
          u.id === editingUser.id
            ? {
                ...u,
                nombre: formData.nombre,
                email: formData.email,
                rol: formData.rol,
                sucursal: formData.sucursal,
              }
            : u
        )
      );
    } else {
      // Crear nuevo usuario
      const newUser = {
        id: String(usuarios.length + 1),
        nombre: formData.nombre,
        email: formData.email,
        password: formData.password,
        rol: formData.rol,
        sucursal: formData.sucursal,
        activo: true,
        fechaCreacion: new Date().toISOString().split('T')[0],
      };
      setUsuarios([...usuarios, newUser]);
    }

    // Reset
    setShowModal(false);
    setEditingUser(null);
    setFormData({
      nombre: '',
      email: '',
      password: '',
      rol: perfiles[0].nombre,
      sucursal: sucursales[0].nombre,
    });
  };

  const handleEdit = (user: any) => {
    setEditingUser(user);
    setFormData({
      nombre: user.nombre,
      email: user.email,
      password: '',
      rol: user.rol,
      sucursal: user.sucursal,
    });
    setShowModal(true);
  };

  const handleToggleActive = (userId: string) => {
    setUsuarios(
      usuarios.map((u) => (u.id === userId ? { ...u, activo: !u.activo } : u))
    );
  };

  const handleDelete = (userId: string) => {
    if (confirm('¿Estás seguro de que deseas eliminar este usuario?')) {
      setUsuarios(usuarios.filter((u) => u.id !== userId));
    }
  };

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
            setEditingUser(null);
            setFormData({
              nombre: '',
              email: '',
              password: '',
              rol: perfiles[0].nombre,
              sucursal: sucursales[0].nombre,
            });
            setShowModal(true);
          }}
          className="flex items-center gap-2 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition"
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
            placeholder="Buscar por nombre o email..."
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
                  Sucursal
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
                <tr key={usuario.id} className="hover:bg-gray-50">
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
                        <div className="text-sm text-gray-500">{usuario.email}</div>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className="px-2 py-1 text-xs font-medium bg-purple-100 text-purple-700 rounded-full">
                      {usuario.rol}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {usuario.sucursal}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <button
                      onClick={() => handleToggleActive(usuario.id)}
                      className={`flex items-center gap-1 px-2 py-1 text-xs font-medium rounded-full ${
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
                    </button>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <button
                      onClick={() => handleEdit(usuario)}
                      className="text-teal-600 hover:text-teal-900 mr-3"
                    >
                      <Edit2 className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => handleDelete(usuario.id)}
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
                  Email
                </label>
                <input
                  type="email"
                  value={formData.email}
                  onChange={(e) =>
                    setFormData({ ...formData, email: e.target.value })
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
                    value={formData.password}
                    onChange={(e) =>
                      setFormData({ ...formData, password: e.target.value })
                    }
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                    required
                  />
                </div>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Rol
                </label>
                <select
                  value={formData.rol}
                  onChange={(e) =>
                    setFormData({ ...formData, rol: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                >
                  {perfiles.map((perfil) => (
                    <option key={perfil.id} value={perfil.nombre}>
                      {perfil.nombre}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Sucursal
                </label>
                <select
                  value={formData.sucursal}
                  onChange={(e) =>
                    setFormData({ ...formData, sucursal: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                >
                  {sucursales.map((sucursal) => (
                    <option key={sucursal.id} value={sucursal.nombre}>
                      {sucursal.nombre}
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
                  className="flex-1 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition"
                >
                  {editingUser ? 'Actualizar' : 'Crear'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
