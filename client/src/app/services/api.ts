import type {
  LoginDto,
  LoginResponseDto,
  UsuarioDto,
  UsuarioDetalleDto,
  CreateUsuarioDto,
  UpdateUsuarioDto,
  AsignarSucursalDto,
  RolDto,
  RolDetalleDto,
  CreateRolDto,
  UpdateRolDto,
  AsignarPermisoDto,
  RolModuloPermisoDto,
  SucursalDto,
  CreateSucursalDto,
  UpdateSucursalDto,
  ProductoDto,
  CreateProductoDto,
  UpdateProductoDto,
  InventarioDto,
  UpdateStockMinimoDto,
  MovimientoInventarioDto,
  CreateMovimientoDto,
  TipoMovimientoDto,
  DashboardDto,
  ModuloDto,
  ModuloCategoriaDto,
} from '../types';

// ─── Base ───

const BASE_URL = '/api';

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE_URL}${url}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });

  if (!res.ok) {
    const body = await res.json().catch(() => null);
    throw new Error(body?.message ?? `Error ${res.status}: ${res.statusText}`);
  }

  // 204 No Content
  if (res.status === 204) return undefined as T;

  return res.json();
}

// ─── Auth ───

export const authApi = {
  login: (dto: LoginDto) =>
    request<LoginResponseDto>('/Usuarios/login', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),
};

// ─── Usuarios ───

export const usuariosApi = {
  getAll: () => request<UsuarioDto[]>('/Usuarios'),

  getById: (id: number) => request<UsuarioDetalleDto>(`/Usuarios/${id}`),

  create: (dto: CreateUsuarioDto) =>
    request<UsuarioDto>('/Usuarios', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),

  update: (id: number, dto: UpdateUsuarioDto) =>
    request<void>(`/Usuarios/${id}`, {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),

  desactivar: (id: number) =>
    request<void>(`/Usuarios/${id}`, { method: 'DELETE' }),

  asignarSucursal: (idUsuario: number, dto: AsignarSucursalDto) =>
    request<void>(`/Usuarios/${idUsuario}/sucursales`, {
      method: 'POST',
      body: JSON.stringify(dto),
    }),

  removerSucursal: (idUsuario: number, idSucursal: number) =>
    request<void>(`/Usuarios/${idUsuario}/sucursales/${idSucursal}`, {
      method: 'DELETE',
    }),
};

// ─── Roles ───

export const rolesApi = {
  getAll: () => request<RolDto[]>('/Roles'),

  getById: (id: number) => request<RolDetalleDto>(`/Roles/${id}`),

  create: (dto: CreateRolDto) =>
    request<RolDto>('/Roles', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),

  update: (id: number, dto: UpdateRolDto) =>
    request<void>(`/Roles/${id}`, {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),

  delete: (id: number) =>
    request<void>(`/Roles/${id}`, { method: 'DELETE' }),

  getPermisos: (id: number) =>
    request<RolModuloPermisoDto[]>(`/Roles/${id}/permisos`),

  asignarPermiso: (id: number, dto: AsignarPermisoDto) =>
    request<void>(`/Roles/${id}/permisos`, {
      method: 'POST',
      body: JSON.stringify(dto),
    }),

  removerPermiso: (idRol: number, idModuloCategoria: number) =>
    request<void>(`/Roles/${idRol}/permisos/${idModuloCategoria}`, {
      method: 'DELETE',
    }),
};

// ─── Módulos ───

export const modulosApi = {
  getAll: () => request<ModuloDto[]>('/Modulos'),

  getCategorias: (idModulo: number) =>
    request<ModuloCategoriaDto[]>(`/Modulos/${idModulo}/categorias`),
};

// ─── Sucursales ───

export const sucursalesApi = {
  getAll: () => request<SucursalDto[]>('/Sucursales'),

  getById: (id: number) => request<SucursalDto>(`/Sucursales/${id}`),

  create: (dto: CreateSucursalDto) =>
    request<SucursalDto>('/Sucursales', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),

  update: (id: number, dto: UpdateSucursalDto) =>
    request<void>(`/Sucursales/${id}`, {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),

  desactivar: (id: number) =>
    request<void>(`/Sucursales/${id}`, { method: 'DELETE' }),
};

// ─── Productos ───

export const productosApi = {
  getAll: () => request<ProductoDto[]>('/Productos'),

  getById: (id: number) => request<ProductoDto>(`/Productos/${id}`),

  create: (dto: CreateProductoDto) =>
    request<ProductoDto>('/Productos', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),

  update: (id: number, dto: UpdateProductoDto) =>
    request<void>(`/Productos/${id}`, {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),

  desactivar: (id: number) =>
    request<void>(`/Productos/${id}`, { method: 'DELETE' }),
};

// ─── Inventario ───

export const inventarioApi = {
  getAll: () => request<InventarioDto[]>('/Inventario'),

  getBySucursal: (idSucursal: number) =>
    request<InventarioDto[]>(`/Inventario/sucursal/${idSucursal}`),

  getByProducto: (idProducto: number) =>
    request<InventarioDto[]>(`/Inventario/producto/${idProducto}`),

  getAlertas: () => request<InventarioDto[]>('/Inventario/alertas'),

  updateStockMinimo: (idInventario: number, dto: UpdateStockMinimoDto) =>
    request<void>(`/Inventario/${idInventario}/stock-minimo`, {
      method: 'PATCH',
      body: JSON.stringify(dto),
    }),
};

// ─── Movimientos ───

export const movimientosApi = {
  getAll: () => request<MovimientoInventarioDto[]>('/Movimientos'),

  getById: (id: number) =>
    request<MovimientoInventarioDto>(`/Movimientos/${id}`),

  getBySucursal: (idSucursal: number) =>
    request<MovimientoInventarioDto[]>(`/Movimientos/sucursal/${idSucursal}`),

  registrar: (dto: CreateMovimientoDto) =>
    request<MovimientoInventarioDto>('/Movimientos', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),
};

// ─── Tipos de Movimiento ───

export const tiposMovimientoApi = {
  getAll: () => request<TipoMovimientoDto[]>('/TiposMovimiento'),
};

// ─── Dashboard ───

export const dashboardApi = {
  getDashboard: () => request<DashboardDto>('/Dashboard'),
};
