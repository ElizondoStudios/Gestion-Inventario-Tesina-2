import { useState, useEffect } from 'react';
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
  Loader2,
} from 'lucide-react';
import { dashboardApi } from '../services/api';
import type { DashboardDto } from '../types';

const COLORS = ['#5EEAD4', '#2DD4BF', '#14B8A6', '#0D9488'];

export function Dashboard() {
  const [data, setData] = useState<DashboardDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    try {
      setLoading(true);
      const result = await dashboardApi.getDashboard();
      setData(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cargar el dashboard');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="w-8 h-8 animate-spin text-teal-500" />
      </div>
    );
  }

  if (error || !data) {
    return (
      <div className="bg-red-50 border border-red-200 text-red-700 px-6 py-4 rounded-xl">
        <p>{error || 'No se pudo cargar el dashboard'}</p>
        <button onClick={loadDashboard} className="mt-2 text-sm underline">
          Reintentar
        </button>
      </div>
    );
  }

  const formatPct = (val?: number) => {
    if (val == null) return '';
    const sign = val >= 0 ? '+' : '';
    return `${sign}${val.toFixed(1)}% vs mes anterior`;
  };

  // Transformar datos para las gráficas
  const chartData = data.ventasVsComprasPorMes.map((m) => {
    const [, mes] = m.mes.split('-');
    const meses: Record<string, string> = {
      '01': 'Ene', '02': 'Feb', '03': 'Mar', '04': 'Abr',
      '05': 'May', '06': 'Jun', '07': 'Jul', '08': 'Ago',
      '09': 'Sep', '10': 'Oct', '11': 'Nov', '12': 'Dic',
    };
    return {
      mes: meses[mes] ?? mes,
      ventas: m.totalVentas,
      compras: m.totalCompras,
    };
  });

  const pieData = data.ventasPorCategoria.map((c) => ({
    categoria: c.categoria,
    valor: c.totalVentas,
  }));

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
                ${data.ventasTotales.montoActual.toLocaleString()}
              </p>
            </div>
            <div className="w-12 h-12 bg-teal-100 rounded-lg flex items-center justify-center">
              <TrendingUp className="w-6 h-6 text-teal-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center gap-1 text-sm text-teal-600">
            <TrendingUp className="w-4 h-4" />
            <span>{formatPct(data.ventasTotales.porcentajeCambio)}</span>
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Compras Totales</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">
                ${data.comprasTotales.montoActual.toLocaleString()}
              </p>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
              <TrendingDown className="w-6 h-6 text-blue-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center gap-1 text-sm text-blue-600">
            <Activity className="w-4 h-4" />
            <span>{formatPct(data.comprasTotales.porcentajeCambio)}</span>
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Productos en Stock</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">
                {data.productosEnStock.cantidadTotal}
              </p>
            </div>
            <div className="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
              <Package className="w-6 h-6 text-purple-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center gap-1 text-sm text-gray-600">
            <span>{data.productosEnStock.cantidadStockBajo} con stock bajo</span>
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm p-6 border border-gray-100">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-gray-600">Merma Total</p>
              <p className="text-2xl font-bold text-gray-900 mt-1">
                ${data.mermaTotal.montoActual.toLocaleString()}
              </p>
            </div>
            <div className="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
              <AlertTriangle className="w-6 h-6 text-red-600" />
            </div>
          </div>
          <div className="mt-4 flex items-center gap-1 text-sm text-red-600">
            <TrendingDown className="w-4 h-4" />
            <span>{formatPct(data.mermaTotal.porcentajeCambio)}</span>
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
            <BarChart data={chartData}>
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
                data={pieData}
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
                {pieData.map((_entry, index) => (
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
            {data.alertasInventario.length === 0 ? (
              <p className="text-gray-500 text-sm">No hay alertas de inventario</p>
            ) : (
              data.alertasInventario.map((alerta) => (
                <div
                  key={alerta.idInventario}
                  className="flex items-center justify-between p-3 bg-orange-50 rounded-lg border border-orange-200"
                >
                  <div>
                    <p className="font-medium text-gray-900">{alerta.nombreProducto}</p>
                    <p className="text-sm text-gray-600">{alerta.nombreSucursal}</p>
                  </div>
                  <div className="text-right">
                    <p className="text-sm font-medium text-orange-600">
                      Stock: {alerta.stockActual}
                    </p>
                    <p className="text-xs text-gray-500">
                      Mínimo: {alerta.stockMinimo}
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
            {data.movimientosRecientes.slice(0, 5).map((movimiento) => {
              const tipo = movimiento.tipoMovimiento.toLowerCase();
              const tipoColors: Record<string, string> = {
                venta: 'bg-green-50 text-green-700 border-green-200',
                compra: 'bg-blue-50 text-blue-700 border-blue-200',
                transferencia: 'bg-purple-50 text-purple-700 border-purple-200',
                merma: 'bg-red-50 text-red-700 border-red-200',
              };

              return (
                <div
                  key={movimiento.idMovimiento}
                  className="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
                >
                  <div>
                    <p className="font-medium text-gray-900">{movimiento.nombreProducto}</p>
                    <p className="text-sm text-gray-600">{movimiento.nombreUsuario}</p>
                  </div>
                  <div className="text-right">
                    <span
                      className={`inline-block px-2 py-1 rounded text-xs font-medium border ${
                        tipoColors[tipo] ?? 'bg-gray-50 text-gray-700 border-gray-200'
                      }`}
                    >
                      {movimiento.tipoMovimiento}
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
