import { useState, useEffect } from 'react';
import { rolesApi } from '../services/api';
import type { RolDto, RolDetalleDto, RolModuloPermisoDto } from '../types';
import { Shield, Plus, Edit2, Trash2, Check, X, Loader2 } from 'lucide-react';
import { useAuth } from '../context/AuthContext';
import { hasPermission } from '../utils/permissions';
import { Dialog, DialogContent } from './ui/dialog';

export function Perfiles() {
  const { user } = useAuth();
  const [roles, setRoles] = useState<RolDto[]>([]);
  const [rolesDetalle, setRolesDetalle] = useState<Record<number, RolDetalleDto>>({});
  const [showModal, setShowModal] = useState(false);
  const [editingRol, setEditingRol] = useState<RolDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const [formData, setFormData] = useState({
    nombre: '',
    descripcion: '',
  });

  const canWriteRoles = hasPermission(user, {
    modulo: 'Seguridad',
    categoria: 'Roles y Permisos',
    accion: 'escribir',
  });

  const canDeleteRoles = hasPermission(user, {
    modulo: 'Seguridad',
    categoria: 'Roles y Permisos',
    accion: 'eliminar',
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const allRoles = await rolesApi.getAll();
      setRoles(allRoles);

      // Cargar detalle de cada rol para ver permisos
      const detalles: Record<number, RolDetalleDto> = {};
      await Promise.all(
        allRoles.map(async (r) => {
          try {
            const detalle = await rolesApi.getById(r.idRol);
            detalles[r.idRol] = detalle;
          } catch { /* skip */ }
        })
      );
      setRolesDetalle(detalles);
    } catch (err) {
      console.error('Error al cargar roles:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!canWriteRoles) {
      alert('No tienes permisos para crear o editar perfiles.');
      return;
    }

    setSaving(true);

    try {
      if (editingRol) {
        await rolesApi.update(editingRol.idRol, {
          nombre: formData.nombre,
          descripcion: formData.descripcion,
        });
      } else {
        await rolesApi.create({
          nombre: formData.nombre,
          descripcion: formData.descripcion,
        });
      }

      setShowModal(false);
      setEditingRol(null);
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
      descripcion: '',
    });
  };

  const handleEdit = (rol: RolDto) => {
    if (!canWriteRoles) {
      alert('No tienes permisos para editar perfiles.');
      return;
    }

    setEditingRol(rol);
    setFormData({
      nombre: rol.nombre,
      descripcion: rol.descripcion ?? '',
    });
    setShowModal(true);
  };

  const handleDelete = async (rolId: number) => {
    if (!canDeleteRoles) {
      alert('No tienes permisos para eliminar perfiles.');
      return;
    }

    if (confirm('¿Estás seguro de que deseas eliminar este perfil?')) {
      try {
        await rolesApi.delete(rolId);
        await loadData();
      } catch (err) {
        alert(err instanceof Error ? err.message : 'Error al eliminar');
      }
    }
  };

  // Agrupar permisos por módulo
  const groupPermisos = (permisos: RolModuloPermisoDto[]) => {
    const grouped: Record<string, RolModuloPermisoDto[]> = {};
    for (const p of permisos) {
      const mod = p.nombreModulo ?? 'Sin módulo';
      if (!grouped[mod]) grouped[mod] = [];
      grouped[mod].push(p);
    }
    return grouped;
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
          <h1 className="text-3xl font-bold text-gray-900">Perfiles de Puesto</h1>
          <p className="text-gray-600 mt-1">
            Gestiona roles y permisos del sistema
          </p>
        </div>
        <button
          onClick={() => {
            if (!canWriteRoles) {
              alert('No tienes permisos para crear perfiles.');
              return;
            }

            setEditingRol(null);
            resetForm();
            setShowModal(true);
          }}
          disabled={!canWriteRoles}
          className="flex items-center gap-2 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition disabled:opacity-50"
        >
          <Plus className="w-5 h-5" />
          Nuevo Perfil
        </button>
      </div>

      {/* Grid de perfiles */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {roles.map((rol) => {
          const detalle = rolesDetalle[rol.idRol];
          const permisos = detalle?.permisos ?? [];
          const grouped = groupPermisos(permisos);

          return (
            <div
              key={rol.idRol}
              className="bg-white rounded-xl shadow-sm border border-gray-100 p-6"
            >
              <div className="flex items-start justify-between mb-4">
                <div className="flex items-center gap-3">
                  <div className="w-12 h-12 bg-teal-100 rounded-lg flex items-center justify-center">
                    <Shield className="w-6 h-6 text-teal-600" />
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900">{rol.nombre}</h3>
                    <p className="text-sm text-gray-500">{rol.descripcion}</p>
                  </div>
                </div>
                <div className="flex gap-2">
                  <button
                    onClick={() => handleEdit(rol)}
                    disabled={!canWriteRoles}
                    className="text-teal-600 hover:text-teal-900 disabled:opacity-40"
                  >
                    <Edit2 className="w-4 h-4" />
                  </button>
                  <button
                    onClick={() => handleDelete(rol.idRol)}
                    disabled={!canDeleteRoles}
                    className="text-red-600 hover:text-red-900 disabled:opacity-40"
                  >
                    <Trash2 className="w-4 h-4" />
                  </button>
                </div>
              </div>

              <div className="space-y-3">
                <h4 className="text-sm font-medium text-gray-700">Permisos:</h4>
                {permisos.length === 0 ? (
                  <p className="text-sm text-gray-400 italic">Sin permisos asignados</p>
                ) : (
                  Object.entries(grouped).map(([modulo, cats]) => (
                    <div key={modulo} className="space-y-1">
                      <p className="text-xs font-semibold text-gray-500 uppercase">{modulo}</p>
                      {cats.map((perm) => (
                        <div
                          key={perm.idModuloCategoria}
                          className="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                        >
                          <span className="text-sm text-gray-700">
                            {perm.nombreCategoria ?? 'General'}
                          </span>
                          <div className="flex gap-2">
                            <span
                              className={`flex items-center gap-1 px-2 py-1 rounded text-xs ${
                                perm.puedeLeer
                                  ? 'bg-green-100 text-green-700'
                                  : 'bg-gray-200 text-gray-500'
                              }`}
                            >
                              {perm.puedeLeer ? <Check className="w-3 h-3" /> : <X className="w-3 h-3" />}
                              Leer
                            </span>
                            <span
                              className={`flex items-center gap-1 px-2 py-1 rounded text-xs ${
                                perm.puedeEscribir
                                  ? 'bg-blue-100 text-blue-700'
                                  : 'bg-gray-200 text-gray-500'
                              }`}
                            >
                              {perm.puedeEscribir ? <Check className="w-3 h-3" /> : <X className="w-3 h-3" />}
                              Escribir
                            </span>
                            <span
                              className={`flex items-center gap-1 px-2 py-1 rounded text-xs ${
                                perm.puedeEliminar
                                  ? 'bg-red-100 text-red-700'
                                  : 'bg-gray-200 text-gray-500'
                              }`}
                            >
                              {perm.puedeEliminar ? <Check className="w-3 h-3" /> : <X className="w-3 h-3" />}
                              Eliminar
                            </span>
                          </div>
                        </div>
                      ))}
                    </div>
                  ))
                )}
              </div>
            </div>
          );
        })}
      </div>

      {/* Modal de creación/edición */}
      <Dialog
        open={showModal}
        onOpenChange={(open) => {
          setShowModal(open);
          if (!open) setEditingRol(null);
        }}
      >
        <DialogContent className="max-w-lg w-full p-6 max-h-[90vh] overflow-y-auto">
            <h2 className="text-xl font-bold text-gray-900 mb-4">
              {editingRol ? 'Editar Perfil' : 'Nuevo Perfil'}
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
                />
              </div>

              <div className="flex gap-3 pt-4">
                <button
                  type="button"
                  onClick={() => {
                    setShowModal(false);
                    setEditingRol(null);
                  }}
                  className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition"
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  disabled={saving || !canWriteRoles}
                  className="flex-1 px-4 py-2 bg-teal-300 hover:bg-teal-400 text-white rounded-lg transition disabled:opacity-50"
                >
                  {saving ? 'Guardando...' : editingRol ? 'Actualizar' : 'Crear'}
                </button>
              </div>
            </form>
        </DialogContent>
      </Dialog>
    </div>
  );
}
