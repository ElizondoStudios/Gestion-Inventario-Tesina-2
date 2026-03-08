import { useState } from 'react';
import { perfiles as perfilesData } from '../data/mockData';
import { Shield, Plus, Edit2, Trash2, Check, X } from 'lucide-react';

export function Perfiles() {
  const [perfiles, setPerfiles] = useState(perfilesData);
  const [showModal, setShowModal] = useState(false);
  const [editingPerfil, setEditingPerfil] = useState<any>(null);

  const [formData, setFormData] = useState({
    nombre: '',
    descripcion: '',
    permisos: {
      usuarios: { leer: false, escribir: false },
      perfiles: { leer: false, escribir: false },
      sucursales: { leer: false, escribir: false },
      inventarios: { leer: false, escribir: false },
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (editingPerfil) {
      setPerfiles(
        perfiles.map((p) =>
          p.id === editingPerfil.id
            ? { ...p, nombre: formData.nombre, descripcion: formData.descripcion, permisos: formData.permisos }
            : p
        )
      );
    } else {
      const newPerfil = {
        id: String(perfiles.length + 1),
        nombre: formData.nombre,
        descripcion: formData.descripcion,
        permisos: formData.permisos,
      };
      setPerfiles([...perfiles, newPerfil]);
    }

    setShowModal(false);
    setEditingPerfil(null);
    resetForm();
  };

  const resetForm = () => {
    setFormData({
      nombre: '',
      descripcion: '',
      permisos: {
        usuarios: { leer: false, escribir: false },
        perfiles: { leer: false, escribir: false },
        sucursales: { leer: false, escribir: false },
        inventarios: { leer: false, escribir: false },
      },
    });
  };

  const handleEdit = (perfil: any) => {
    setEditingPerfil(perfil);
    setFormData({
      nombre: perfil.nombre,
      descripcion: perfil.descripcion,
      permisos: perfil.permisos,
    });
    setShowModal(true);
  };

  const handleDelete = (perfilId: string) => {
    if (confirm('¿Estás seguro de que deseas eliminar este perfil?')) {
      setPerfiles(perfiles.filter((p) => p.id !== perfilId));
    }
  };

  const togglePermiso = (modulo: string, tipo: 'leer' | 'escribir') => {
    setFormData({
      ...formData,
      permisos: {
        ...formData.permisos,
        [modulo]: {
          ...formData.permisos[modulo as keyof typeof formData.permisos],
          [tipo]: !formData.permisos[modulo as keyof typeof formData.permisos][tipo],
        },
      },
    });
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Perfiles de Puesto</h1>
          <p className="text-gray-600 mt-1">
            Gestiona roles y permisos del sistema
          </p>
        </div>
        <button
          onClick={() => {
            setEditingPerfil(null);
            resetForm();
            setShowModal(true);
          }}
          className="flex items-center gap-2 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition"
        >
          <Plus className="w-5 h-5" />
          Nuevo Perfil
        </button>
      </div>

      {/* Grid de perfiles */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {perfiles.map((perfil) => (
          <div
            key={perfil.id}
            className="bg-white rounded-xl shadow-sm border border-gray-100 p-6"
          >
            <div className="flex items-start justify-between mb-4">
              <div className="flex items-center gap-3">
                <div className="w-12 h-12 bg-teal-100 rounded-lg flex items-center justify-center">
                  <Shield className="w-6 h-6 text-teal-600" />
                </div>
                <div>
                  <h3 className="font-semibold text-gray-900">{perfil.nombre}</h3>
                  <p className="text-sm text-gray-500">{perfil.descripcion}</p>
                </div>
              </div>
              <div className="flex gap-2">
                <button
                  onClick={() => handleEdit(perfil)}
                  className="text-teal-600 hover:text-teal-900"
                >
                  <Edit2 className="w-4 h-4" />
                </button>
                <button
                  onClick={() => handleDelete(perfil.id)}
                  className="text-red-600 hover:text-red-900"
                >
                  <Trash2 className="w-4 h-4" />
                </button>
              </div>
            </div>

            <div className="space-y-3">
              <h4 className="text-sm font-medium text-gray-700">Permisos:</h4>
              {Object.entries(perfil.permisos).map(([modulo, permisos]) => (
                <div
                  key={modulo}
                  className="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                >
                  <span className="text-sm text-gray-700 capitalize">
                    {modulo}
                  </span>
                  <div className="flex gap-3">
                    <span
                      className={`flex items-center gap-1 px-2 py-1 rounded text-xs ${
                        permisos.leer
                          ? 'bg-green-100 text-green-700'
                          : 'bg-gray-200 text-gray-500'
                      }`}
                    >
                      {permisos.leer ? (
                        <Check className="w-3 h-3" />
                      ) : (
                        <X className="w-3 h-3" />
                      )}
                      Leer
                    </span>
                    <span
                      className={`flex items-center gap-1 px-2 py-1 rounded text-xs ${
                        permisos.escribir
                          ? 'bg-blue-100 text-blue-700'
                          : 'bg-gray-200 text-gray-500'
                      }`}
                    >
                      {permisos.escribir ? (
                        <Check className="w-3 h-3" />
                      ) : (
                        <X className="w-3 h-3" />
                      )}
                      Escribir
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>

      {/* Modal de creación/edición */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl max-w-lg w-full p-6 max-h-[90vh] overflow-y-auto">
            <h2 className="text-xl font-bold text-gray-900 mb-4">
              {editingPerfil ? 'Editar Perfil' : 'Nuevo Perfil'}
            </h2>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Nombre del Perfil
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
                  Descripción
                </label>
                <textarea
                  value={formData.descripcion}
                  onChange={(e) =>
                    setFormData({ ...formData, descripcion: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  rows={3}
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-3">
                  Permisos por Módulo
                </label>
                <div className="space-y-3">
                  {Object.entries(formData.permisos).map(([modulo, permisos]) => (
                    <div
                      key={modulo}
                      className="p-4 border border-gray-200 rounded-lg"
                    >
                      <p className="font-medium text-gray-900 mb-3 capitalize">
                        {modulo}
                      </p>
                      <div className="flex gap-4">
                        <label className="flex items-center gap-2 cursor-pointer">
                          <input
                            type="checkbox"
                            checked={permisos.leer}
                            onChange={() => togglePermiso(modulo, 'leer')}
                            className="w-4 h-4 text-teal-600 border-gray-300 rounded focus:ring-teal-300"
                          />
                          <span className="text-sm text-gray-700">Leer</span>
                        </label>
                        <label className="flex items-center gap-2 cursor-pointer">
                          <input
                            type="checkbox"
                            checked={permisos.escribir}
                            onChange={() => togglePermiso(modulo, 'escribir')}
                            className="w-4 h-4 text-teal-600 border-gray-300 rounded focus:ring-teal-300"
                          />
                          <span className="text-sm text-gray-700">Escribir</span>
                        </label>
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              <div className="flex gap-3 pt-4">
                <button
                  type="button"
                  onClick={() => {
                    setShowModal(false);
                    setEditingPerfil(null);
                  }}
                  className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  className="flex-1 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition"
                >
                  {editingPerfil ? 'Actualizar' : 'Crear'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
