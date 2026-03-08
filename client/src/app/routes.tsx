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
        element: <Usuarios />,
      },
      {
        path: 'perfiles',
        element: <Perfiles />,
      },
      {
        path: 'sucursales',
        element: <Sucursales />,
      },
      {
        path: 'inventarios',
        element: <Inventarios />,
      },
    ],
  },
  {
    path: '*',
    element: <Navigate to="/" replace />,
  },
]);
