import { ReactNode, useState } from 'react';
import { Link, useLocation, Outlet } from 'react-router';
import { useAuth } from '../context/AuthContext';
import {
  LayoutDashboard,
  Users,
  Shield,
  Building2,
  Package,
  LogOut,
  Menu,
  X,
} from 'lucide-react';
import { hasAnyPermission, type PermissionRequirement } from '../utils/permissions';

const navigation = [
  {
    name: 'Dashboard',
    href: '/',
    icon: LayoutDashboard,
    requiredAny: [{ modulo: 'Dashboard', categoria: 'Vista General', accion: 'leer' }] as PermissionRequirement[],
  },
  {
    name: 'Usuarios',
    href: '/usuarios',
    icon: Users,
    requiredAny: [{ modulo: 'Seguridad', categoria: 'Usuarios', accion: 'leer' }] as PermissionRequirement[],
  },
  {
    name: 'Perfiles de Puesto',
    href: '/perfiles',
    icon: Shield,
    requiredAny: [{ modulo: 'Seguridad', categoria: 'Roles y Permisos', accion: 'leer' }] as PermissionRequirement[],
  },
  {
    name: 'Sucursales',
    href: '/sucursales',
    icon: Building2,
    requiredAny: [{ modulo: 'Inventario', categoria: 'Inventarios', accion: 'leer' }] as PermissionRequirement[],
  },
  {
    name: 'Inventarios',
    href: '/inventarios',
    icon: Package,
    requiredAny: [
      { modulo: 'Inventario', categoria: 'Productos', accion: 'leer' },
      { modulo: 'Inventario', categoria: 'Inventarios', accion: 'leer' },
      { modulo: 'Inventario', categoria: 'Movimientos', accion: 'leer' },
    ] as PermissionRequirement[],
  },
];

export function Layout() {
  const { user, logout } = useAuth();
  const location = useLocation();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const visibleNavigation = navigation.filter((item) => hasAnyPermission(user, item.requiredAny));

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Sidebar para móvil */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 bg-black/30 z-40 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed top-0 left-0 z-50 h-full w-64 bg-white shadow-xl transform transition-transform duration-300 lg:translate-x-0 ${
          sidebarOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        <div className="flex flex-col h-full">
          {/* Header */}
          <div className="p-6 bg-gradient-to-r from-teal-300 to-teal-400">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-white rounded-lg flex items-center justify-center">
                  <Package className="w-6 h-6 text-teal-400" />
                </div>
                <div>
                  <h1 className="font-bold text-white">Inventario</h1>
                  <p className="text-xs text-teal-50">Sistema de Gestión</p>
                </div>
              </div>
              <button
                onClick={() => setSidebarOpen(false)}
                className="lg:hidden text-white"
              >
                <X className="w-6 h-6" />
              </button>
            </div>
          </div>

          {/* Usuario */}
          <div className="p-4 border-b">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-teal-100 rounded-full flex items-center justify-center">
                <span className="text-teal-700 font-medium">
                  {user?.nombre.charAt(0)}
                </span>
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-sm font-medium text-gray-900 truncate">
                  {user?.nombre}
                </p>
                <p className="text-xs text-gray-500 truncate">{user?.nombreRol}</p>
              </div>
            </div>
          </div>

          {/* Navegación */}
          <nav className="flex-1 p-4 space-y-1 overflow-y-auto">
            {visibleNavigation.map((item) => {
              const isActive = location.pathname === item.href;
              return (
                <Link
                  key={item.name}
                  to={item.href}
                  onClick={() => setSidebarOpen(false)}
                  className={`flex items-center gap-3 px-4 py-3 rounded-lg transition ${
                    isActive
                      ? 'bg-teal-50 text-teal-700 font-medium'
                      : 'text-gray-700 hover:bg-gray-50'
                  }`}
                >
                  <item.icon className="w-5 h-5" />
                  <span>{item.name}</span>
                </Link>
              );
            })}
          </nav>

          {/* Logout */}
          <div className="p-4 border-t">
            <button
              onClick={logout}
              className="flex items-center gap-3 px-4 py-3 rounded-lg text-red-600 hover:bg-red-50 w-full transition"
            >
              <LogOut className="w-5 h-5" />
              <span>Cerrar Sesión</span>
            </button>
          </div>
        </div>
      </aside>

      {/* Contenido principal */}
      <div className="lg:pl-64">
        {/* Header móvil */}
        <header className="bg-white shadow-sm lg:hidden sticky top-0 z-30">
          <div className="flex items-center justify-between p-4">
            <button
              onClick={() => setSidebarOpen(true)}
              className="text-gray-700"
            >
              <Menu className="w-6 h-6" />
            </button>
            <h2 className="font-medium text-gray-900">Sistema de Inventario</h2>
            <div className="w-6" />
          </div>
        </header>

        {/* Contenido */}
        <main className="p-4 lg:p-8">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
