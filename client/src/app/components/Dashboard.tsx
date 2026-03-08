import { useState } from 'react';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
} from 'recharts';
import {
  TrendingUp,
  TrendingDown,
  Package,
  AlertTriangle,
  Activity,
} from 'lucide-react';
import {
  ventasComprasData,
  distribucionVentas,
  movimientos,
  productos,
} from '../data/mockData';

const COLORS = ['#5EEAD4', '#2DD4BF', '#14B8A6', '#0D9488'];

export function Dashboard() {
  const alertasInventario = productos.filter((p) => p.stock < p.minimo);
  const totalVentas = ventasComprasData.reduce((sum, item) => sum + item.ventas, 0);
  const totalCompras = ventasComprasData.reduce((sum, item) => sum + item.compras, 0);
  const totalMerma = movimientos
    .filter((m) => m.tipo === 'merma')
    .reduce((sum, m) => sum + m.cantidad, 0);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard de Inventarios</h1>
        <p className="text-gray-600 mt-1">Vista general del sistema</p>
      </div>

      {/* Cards de métricas */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Ventas Totales</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">
                ${totalVentas.toLocaleString()}
              </p>
            </div>
            <div className="w-12 h-12 bg-teal-100 rounded-lg flex items-center justify-center">
              <TrendingUp className="w-6 h-6 text-teal-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center gap-1 text-sm text-teal-600">
            <TrendingUp className="w-4 h-4" />
            <span>+12.5% vs mes anterior</span>
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Compras Totales</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">
                ${totalCompras.toLocaleString()}
              </p>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <TrendingDown className="w-6 h-6 text-blue-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center gap-1 text-sm text-blue-600">
            <Activity className="w-4 h-4" />
            <span>+8.2% vs mes anterior</span>
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Productos en Stock</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">
                {productos.length}
              </p>
            </div>
            <div className="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
              <Package className="w-6 h-6 text-purple-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center gap-1 text-sm text-gray-600">
            <span>{alertasInventario.length} con stock bajo</span>
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Merma Total</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">{totalMerma}</p>
            </div>
            <div className="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
              <AlertTriangle className="w-6 h-6 text-red-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center gap-1 text-sm text-red-600">
            <TrendingDown className="w-4 h-4" />
            <span>-3.1% vs mes anterior</span>
          </div>
        </div>
      </div>

      {/* Gráficas */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Ventas vs Compras */}
        <div className="lg:col-span-2 bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">
            Ventas vs Compras
          </h2>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={ventasComprasData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
              <XAxis dataKey="mes" stroke="#6b7280" />
              <YAxis stroke="#6b7280" />
              <Tooltip
                contentStyle={{
                  backgroundColor: 'white',
                  border: '1px solid #e5e7eb',
                  borderRadius: '8px',
                }}
              />
              <Legend />
              <Bar dataKey="ventas" fill="#5EEAD4" name="Ventas" radius={[8, 8, 0, 0]} />
              <Bar dataKey="compras" fill="#14B8A6" name="Compras" radius={[8, 8, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>

        {/* Distribución por Categoría */}
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">
            Distribución por Categoría
          </h2>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={distribucionVentas}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ categoria, percent }) =>
                  `${categoria} ${(percent * 100).toFixed(0)}%`
                }
                outerRadius={80}
                fill="#8884d8"
                dataKey="valor"
              >
                {distribucionVentas.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Alertas y Movimientos */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Alertas de Inventario */}
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center gap-2 mb-4">
            <AlertTriangle className="w-5 h-5 text-orange-500" />
            <h2 className="text-lg font-semibold text-gray-900">
              Alertas de Inventario
            </h2>
          </div>
          <div className="space-y-3">
            {alertasInventario.length === 0 ? (
              <p className="text-gray-500 text-sm">No hay alertas de inventario</p>
            ) : (
              alertasInventario.map((producto) => (
                <div
                  key={producto.id}
                  className="flex items-center justify-between p-3 bg-orange-50 rounded-lg border border-orange-200"
                >
                  <div>
                    <p className="font-medium text-gray-900">{producto.nombre}</p>
                    <p className="text-sm text-gray-600">{producto.sucursal}</p>
                  </div>
                  <div className="text-right">
                    <p className="text-sm font-medium text-orange-600">
                      Stock: {producto.stock}
                    </p>
                    <p className="text-xs text-gray-500">
                      Mínimo: {producto.minimo}
                    </p>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        {/* Movimientos Recientes */}
        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center gap-2 mb-4">
            <Activity className="w-5 h-5 text-teal-500" />
            <h2 className="text-lg font-semibold text-gray-900">
              Movimientos Recientes
            </h2>
          </div>
          <div className="space-y-3">
            {movimientos.slice(0, 5).map((movimiento) => {
              const tipoColors = {
                entrada: 'bg-green-50 text-green-700 border-green-200',
                salida: 'bg-blue-50 text-blue-700 border-blue-200',
                transferencia: 'bg-purple-50 text-purple-700 border-purple-200',
                merma: 'bg-red-50 text-red-700 border-red-200',
              };

              return (
                <div
                  key={movimiento.id}
                  className="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                >
                  <div>
                    <p className="font-medium text-gray-900">{movimiento.producto}</p>
                    <p className="text-sm text-gray-600">{movimiento.usuario}</p>
                  </div>
                  <div className="text-right">
                    <span
                      className={`inline-block px-2 py-1 rounded text-xs font-medium border ${
                        tipoColors[movimiento.tipo]
                      }`}
                    >
                      {movimiento.tipo.charAt(0).toUpperCase() + movimiento.tipo.slice(1)}
                    </span>
                    <p className="text-xs text-gray-500 mt-1">
                      {movimiento.cantidad} unidades
                    </p>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      </div>
    </div>
  );
}
