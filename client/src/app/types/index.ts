// ─── Tipos que mapean a los DTOs del backend .NET ───

// ─── Usuarios ───

export interface UsuarioDto {
  idUsuario: number;
  nombre: string;
  correo: string;
  activo: boolean;
  idRol: number;
  nombreRol?: string;
}

export interface UsuarioDetalleDto {
  idUsuario: number;
  nombre: string;
  correo: string;
  activo: boolean;
  idRol: number;
  nombreRol?: string;
  sucursales: SucursalDto[];
}

export interface LoginResponseDto {
  idUsuario: number;
  nombre: string;
  correo: string;
  nombreRol: string;
  permisos: RolModuloPermisoDto[];
}

export interface CreateUsuarioDto {
  nombre: string;
  correo: string;
  contrasenia: string;
  idRol: number;
}

export interface UpdateUsuarioDto {
  nombre: string;
  correo: string;
  idRol: number;
}

export interface LoginDto {
  correo: string;
  contrasenia: string;
}

export interface AsignarSucursalDto {
  idSucursal: number;
}

// ─── Roles ───

export interface RolDto {
  idRol: number;
  nombre: string;
  descripcion?: string;
}

export interface RolDetalleDto {
  idRol: number;
  nombre: string;
  descripcion?: string;
  permisos: RolModuloPermisoDto[];
}

export interface CreateRolDto {
  nombre: string;
  descripcion?: string;
}

export interface UpdateRolDto {
  nombre: string;
  descripcion?: string;
}

// ─── Módulos y Permisos ───

export interface ModuloDto {
  idModulo: number;
  nombre: string;
  descripcion?: string;
  activo: boolean;
}

export interface ModuloCategoriaDto {
  idModuloCategoria: number;
  idModulo: number;
  nombre: string;
  descripcion?: string;
  nombreModulo?: string;
}

export interface RolModuloPermisoDto {
  idRol: number;
  idModuloCategoria: number;
  nombreModulo?: string;
  nombreCategoria?: string;
  puedeLeer: boolean;
  puedeEscribir: boolean;
  puedeEliminar: boolean;
}

export interface AsignarPermisoDto {
  idModuloCategoria: number;
  puedeLeer: boolean;
  puedeEscribir: boolean;
  puedeEliminar: boolean;
}

// ─── Sucursales ───

export interface SucursalDto {
  idSucursal: number;
  nombre: string;
  direccion: string;
  ciudad: string;
  estado: string;
  activa: boolean;
}

export interface CreateSucursalDto {
  nombre: string;
  direccion: string;
  ciudad: string;
  estado: string;
}

export interface UpdateSucursalDto {
  nombre: string;
  direccion: string;
  ciudad: string;
  estado: string;
}

// ─── Productos ───

export interface ProductoDto {
  idProducto: number;
  nombre: string;
  descripcion?: string;
  numeroParte: string;
  precio: number;
  activo: boolean;
}

export interface CreateProductoDto {
  nombre: string;
  descripcion?: string;
  numeroParte: string;
  precio: number;
}

export interface UpdateProductoDto {
  nombre: string;
  descripcion?: string;
  numeroParte: string;
  precio: number;
}

// ─── Inventario ───

export interface InventarioDto {
  idInventario: number;
  idProducto: number;
  nombreProducto?: string;
  numeroParte?: string;
  idSucursal: number;
  nombreSucursal?: string;
  stockActual: number;
  stockMinimo: number;
  stockBajo: boolean;
}

export interface UpdateStockMinimoDto {
  stockMinimo: number;
}

// ─── Movimientos ───

export interface MovimientoInventarioDto {
  idMovimiento: number;
  idProducto: number;
  nombreProducto?: string;
  idSucursalOrigen?: number;
  nombreSucursalOrigen?: string;
  idSucursalDestino?: number;
  nombreSucursalDestino?: string;
  idUsuario: number;
  nombreUsuario?: string;
  idTipoMovimiento: number;
  nombreTipoMovimiento?: string;
  cantidad: number;
  fecha: string;
  observaciones?: string;
}

export interface CreateMovimientoDto {
  idProducto: number;
  idSucursalOrigen?: number;
  idSucursalDestino?: number;
  idUsuario: number;
  idTipoMovimiento: number;
  cantidad: number;
  observaciones?: string;
}

// ─── Tipos de Movimiento ───

export interface TipoMovimientoDto {
  idTipoMovimiento: number;
  nombre: string;
  descripcion?: string;
  afectaStock: boolean;
  esTransferencia: boolean;
}

// ─── Dashboard ───

export interface DashboardDto {
  ventasTotales: ResumenTarjetaDto;
  comprasTotales: ResumenTarjetaDto;
  productosEnStock: ResumenStockDto;
  mermaTotal: ResumenTarjetaDto;
  ventasVsComprasPorMes: VentasVsComprasMesDto[];
  ventasPorCategoria: VentasPorCategoriaDto[];
  alertasInventario: AlertaInventarioDto[];
  movimientosRecientes: MovimientoRecienteDto[];
}

export interface ResumenTarjetaDto {
  montoActual: number;
  montoAnterior: number;
  porcentajeCambio?: number;
}

export interface ResumenStockDto {
  cantidadTotal: number;
  cantidadStockBajo: number;
}

export interface VentasVsComprasMesDto {
  mes: string;
  totalVentas: number;
  totalCompras: number;
}

export interface VentasPorCategoriaDto {
  categoria: string;
  totalVentas: number;
}

export interface AlertaInventarioDto {
  idInventario: number;
  nombreProducto: string;
  numeroParte: string;
  nombreSucursal: string;
  stockActual: number;
  stockMinimo: number;
}

export interface MovimientoRecienteDto {
  idMovimiento: number;
  nombreProducto: string;
  tipoMovimiento: string;
  cantidad: number;
  sucursalOrigen?: string;
  sucursalDestino?: string;
  nombreUsuario: string;
  fecha: string;
}
