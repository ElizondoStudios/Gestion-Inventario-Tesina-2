import { useState, useEffect } from 'react';
import { sucursalesApi } from '../services/api';
import type { SucursalDto } from '../types';
import { Building2, Plus, Search, Edit2, Trash2, MapPin, Loader2 } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { hasPermission } from '../utils/permissions';
import { Dialog, DialogContent } from './ui/dialog';

export function Sucursales() {
  const { user } = useAuth();
  const [sucursales, setSucursales] = useState<SucursalDto[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingSucursal, setEditingSucursal] = useState<SucursalDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const [formData, setFormData] = useState({
    nombre: '',
    direccion: '',
    ciudad: '',
    estado: '',
  });

  const canWriteSucursales = hasPermission(user, {
    modulo: 'Inventario',
    categoria: 'Inventarios',
    accion: 'escribir',
  });

  const canDeleteSucursales = hasPermission(user, {
    modulo: 'Inventario',
    categoria: 'Inventarios',
    accion: 'eliminar',
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await sucursalesApi.getAll();
      setSucursales(data);
    } catch (err) {
      console.error('Error al cargar sucursales:', err);
    } finally {
      setLoading(false);
    }
  };

  const filteredSucursales = sucursales.filter(
    (sucursal) =>
      sucursal.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
      sucursal.ciudad.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!canWriteSucursales) {
      alert('No tienes permisos para crear o editar sucursales.');
      return;
    }

    setSaving(true);

    try {
      if (editingSucursal) {
        await sucursalesApi.update(editingSucursal.idSucursal, formData);
      } else {
        await sucursalesApi.create(formData);
      }

      setShowModal(false);
      setEditingSucursal(null);
      resetForm();
      await loadData();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Error al guardar');
    } finally {
      setSaving(false);
    }
  };

  const resetForm = () => {
    setFormData({
      nombre: '',
      direccion: '',
      ciudad: '',
      estado: '',
    });
  };

  const handleEdit = (sucursal: SucursalDto) => {
    if (!canWriteSucursales) {
      alert('No tienes permisos para editar sucursales.');
      return;
    }

    setEditingSucursal(sucursal);
    setFormData({
      nombre: sucursal.nombre,
      direccion: sucursal.direccion,
      ciudad: sucursal.ciudad,
      estado: sucursal.estado,
    });
    setShowModal(true);
  };

  const handleDelete = async (sucursalId: number) => {
    if (!canDeleteSucursales) {
      alert('No tienes permisos para desactivar sucursales.');
      return;
    }

    if (confirm('¿Estás seguro de que deseas desactivar esta sucursal?')) {
      try {
        await sucursalesApi.desactivar(sucursalId);
        await loadData();
      } catch (err) {
        alert(err instanceof Error ? err.message : 'Error al eliminar');
      }
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
          <h1 className="text-3xl font-bold text-gray-900">Gestión de Sucursales</h1>
          <p className="text-gray-600 mt-1">
            Administra las ubicaciones de la empresa
          </p>
        </div>
        <button
          onClick={() => {
            if (!canWriteSucursales) {
              alert('No tienes permisos para crear sucursales.');
              return;
            }

            setEditingSucursal(null);
            resetForm();
            setShowModal(true);
          }}
          disabled={!canWriteSucursales}
          className="flex items-center gap-2 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition disabled:opacity-50"
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
            key={sucursal.idSucursal}
            className="bg-white rounded-xl shadow-sm border border-gray-100 p-6"
          >
            <div className="flex items-start justify-between mb-4">
              <div className="flex items-center gap-3">
                <div className="w-12 h-12 bg-teal-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Building2 className="w-6 h-6 text-teal-600" />
                </div>
                <div>
                  <h3 className="font-semibold text-gray-900">{sucursal.nombre}</h3>
                  <span
                    className={`text-xs px-2 py-1 rounded-full mt-1 inline-block ${
                      sucursal.activa
                        ? 'bg-green-100 text-green-700'
                        : 'bg-red-100 text-red-700'
                    }`}
                  >
                    {sucursal.activa ? 'Activa' : 'Inactiva'}
                  </span>
                </div>
              </div>
              <div className="flex gap-2">
                <button
                  onClick={() => handleEdit(sucursal)}
                  disabled={!canWriteSucursales}
                  className="text-teal-600 hover:text-teal-900 disabled:opacity-40"
                >
                  <Edit2 className="w-4 h-4" />
                </button>
                <button
                  onClick={() => handleDelete(sucursal.idSucursal)}
                  disabled={!canDeleteSucursales}
                  className="text-red-600 hover:text-red-900 disabled:opacity-40"
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
                  <p>{sucursal.ciudad}, {sucursal.estado}</p>
                </div>
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
      <Dialog
        open={showModal}
        onOpenChange={(open) => {
          setShowModal(open);
          if (!open) setEditingSucursal(null);
        }}
      >
        <DialogContent className="max-w-md w-full p-6">
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
                  Estado
                </label>
                <input
                  type="text"
                  value={formData.estado}
                  onChange={(e) =>
                    setFormData({ ...formData, estado: e.target.value })
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
                  disabled={saving || !canWriteSucursales}
                  className="flex-1 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition disabled:opacity-50"
                >
                  {saving ? 'Guardando...' : editingSucursal ? 'Actualizar' : 'Crear'}
                </button>
              </div>
            </form>
        </DialogContent>
      </Dialog>
    </div>
  );
}
