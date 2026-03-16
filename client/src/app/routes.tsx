import { createBrowserRouter, Navigate } from 'react-router';
import { Login } from './components/Login';
import { Layout } from './components/Layout';
import { Dashboard } from './components/Dashboard';
import { Usuarios } from './components/Usuarios';
import { Perfiles } from './components/Perfiles';
import { Sucursales } from './components/Sucursales';
import { Inventarios } from './components/Inventarios';
import { ProtectedRoute } from './components/ProtectedRoute';

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <Login />,
  },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <Layout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <Dashboard />,
      },
      {
        path: 'usuarios',
        element: (
          <ProtectedRoute
            requiredAny={[
              { modulo: 'Seguridad', categoria: 'Usuarios', accion: 'leer' },
            ]}
          >
            <Usuarios />
          </ProtectedRoute>
        ),
      },
      {
        path: 'perfiles',
        element: (
          <ProtectedRoute
            requiredAny={[
              { modulo: 'Seguridad', categoria: 'Roles y Permisos', accion: 'leer' },
            ]}
          >
            <Perfiles />
          </ProtectedRoute>
        ),
      },
      {
        path: 'sucursales',
        element: (
          <ProtectedRoute
            requiredAny={[
              { modulo: 'Inventario', categoria: 'Inventarios', accion: 'leer' },
            ]}
          >
            <Sucursales />
          </ProtectedRoute>
        ),
      },
      {
        path: 'inventarios',
        element: (
          <ProtectedRoute
            requiredAny={[
              { modulo: 'Inventario', categoria: 'Productos', accion: 'leer' },
              { modulo: 'Inventario', categoria: 'Inventarios', accion: 'leer' },
              { modulo: 'Inventario', categoria: 'Movimientos', accion: 'leer' },
            ]}
          >
            <Inventarios />
          </ProtectedRoute>
        ),
      },
    ],
  },
  {
    path: '*',
    element: <Navigate to="/" replace />,
  },
]);
