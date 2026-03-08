import { useState } from 'react';
import { sucursales as sucursalesData } from '../data/mockData';
import { Building2, Plus, Search, Edit2, Trash2, MapPin, Phone, User } from 'lucide-react';

export function Sucursales() {
  const [sucursales, setSucursales] = useState(sucursalesData);
  const [searchTerm, setSearchTerm] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingSucursal, setEditingSucursal] = useState<any>(null);

  const [formData, setFormData] = useState({
    nombre: '',
    direccion: '',
    ciudad: '',
    telefono: '',
    gerente: '',
  });

  const filteredSucursales = sucursales.filter(
    (sucursal) =>
      sucursal.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
      sucursal.ciudad.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (editingSucursal) {
      setSucursales(
        sucursales.map((s) =>
          s.id === editingSucursal.id ? { ...s, ...formData } : s
        )
      );
    } else {
      const newSucursal = {
        id: String(sucursales.length + 1),
        ...formData,
        activa: true,
      };
      setSucursales([...sucursales, newSucursal]);
    }

    setShowModal(false);
    setEditingSucursal(null);
    resetForm();
  };

  const resetForm = () => {
    setFormData({
      nombre: '',
      direccion: '',
      ciudad: '',
      telefono: '',
      gerente: '',
    });
  };

  const handleEdit = (sucursal: any) => {
    setEditingSucursal(sucursal);
    setFormData({
      nombre: sucursal.nombre,
      direccion: sucursal.direccion,
      ciudad: sucursal.ciudad,
      telefono: sucursal.telefono,
      gerente: sucursal.gerente,
    });
    setShowModal(true);
  };

  const handleDelete = (sucursalId: string) => {
    if (confirm('¿Estás seguro de que deseas eliminar esta sucursal?')) {
      setSucursales(sucursales.filter((s) => s.id !== sucursalId));
    }
  };

  const handleToggleActive = (sucursalId: string) => {
    setSucursales(
      sucursales.map((s) =>
        s.id === sucursalId ? { ...s, activa: !s.activa } : s
      )
    );
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Gestión de Sucursales</h1>
          <p className="text-gray-600 mt-1">
            Administra las ubicaciones de la empresa
          </p>
        </div>
        <button
          onClick={() => {
            setEditingSucursal(null);
            resetForm();
            setShowModal(true);
          }}
          className="flex items-center gap-2 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition"
        >
          <Plus className="w-5 h-5" />
          Nueva Sucursal
        </button>
      </div>

      {/* Búsqueda */}
      <div className="bg-white rounded-xl shadow-sm p-4 border border-gray-100">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
          <input
            type="text"
            placeholder="Buscar por nombre o ciudad..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
          />
        </div>
      </div>

      {/* Grid de sucursales */}
      <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
        {filteredSucursales.map((sucursal) => (
          <div
            key={sucursal.id}
            className="bg-white rounded-xl shadow-sm border border-gray-100 p-6"
          >
            <div className="flex items-start justify-between mb-4">
              <div className="flex items-center gap-3">
                <div className="w-12 h-12 bg-teal-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Building2 className="w-6 h-6 text-teal-600" />
                </div>
                <div>
                  <h3 className="font-semibold text-gray-900">{sucursal.nombre}</h3>
                  <button
                    onClick={() => handleToggleActive(sucursal.id)}
                    className={`text-xs px-2 py-1 rounded-full mt-1 ${
                      sucursal.activa
                        ? 'bg-green-100 text-green-700'
                        : 'bg-red-100 text-red-700'
                    }`}
                  >
                    {sucursal.activa ? 'Activa' : 'Inactiva'}
                  </button>
                </div>
              </div>
              <div className="flex gap-2">
                <button
                  onClick={() => handleEdit(sucursal)}
                  className="text-teal-600 hover:text-teal-900"
                >
                  <Edit2 className="w-4 h-4" />
                </button>
                <button
                  onClick={() => handleDelete(sucursal.id)}
                  className="text-red-600 hover:text-red-900"
                >
                  <Trash2 className="w-4 h-4" />
                </button>
              </div>
            </div>

            <div className="space-y-3">
              <div className="flex items-start gap-2 text-sm text-gray-600">
                <MapPin className="w-4 h-4 flex-shrink-0 mt-0.5" />
                <div>
                  <p>{sucursal.direccion}</p>
                  <p>{sucursal.ciudad}</p>
                </div>
              </div>
              <div className="flex items-center gap-2 text-sm text-gray-600">
                <Phone className="w-4 h-4 flex-shrink-0" />
                <p>{sucursal.telefono}</p>
              </div>
              <div className="flex items-center gap-2 text-sm text-gray-600">
                <User className="w-4 h-4 flex-shrink-0" />
                <p>Gerente: {sucursal.gerente}</p>
              </div>
            </div>
          </div>
        ))}
      </div>

      {filteredSucursales.length === 0 && (
        <div className="text-center py-12">
          <Building2 className="w-16 h-16 text-gray-300 mx-auto mb-4" />
          <p className="text-gray-500">No se encontraron sucursales</p>
        </div>
      )}

      {/* Modal de creación/edición */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl max-w-md w-full p-6">
            <h2 className="text-xl font-bold text-gray-900 mb-4">
              {editingSucursal ? 'Editar Sucursal' : 'Nueva Sucursal'}
            </h2>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Nombre de la Sucursal
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
                  Dirección
                </label>
                <input
                  type="text"
                  value={formData.direccion}
                  onChange={(e) =>
                    setFormData({ ...formData, direccion: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Ciudad
                </label>
                <input
                  type="text"
                  value={formData.ciudad}
                  onChange={(e) =>
                    setFormData({ ...formData, ciudad: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Teléfono
                </label>
                <input
                  type="tel"
                  value={formData.telefono}
                  onChange={(e) =>
                    setFormData({ ...formData, telefono: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Gerente
                </label>
                <input
                  type="text"
                  value={formData.gerente}
                  onChange={(e) =>
                    setFormData({ ...formData, gerente: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-teal-300 focus:border-transparent"
                  required
                />
              </div>

              <div className="flex gap-3 pt-4">
                <button
                  type="button"
                  onClick={() => {
                    setShowModal(false);
                    setEditingSucursal(null);
                  }}
                  className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  className="flex-1 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition"
                >
                  {editingSucursal ? 'Actualizar' : 'Crear'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
