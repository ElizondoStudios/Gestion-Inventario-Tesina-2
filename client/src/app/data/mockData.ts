// Datos mock para el sistema de inventarios

export interface Usuario {
  id: string;
  nombre: string;
  email: string;
  password?: string;
  rol: string;
  sucursal: string;
  activo: boolean;
  fechaCreacion: string;
}

export interface Perfil {
  id: string;
  nombre: string;
  descripcion: string;
  permisos: {
    usuarios: { leer: boolean; escribir: boolean };
    perfiles: { leer: boolean; escribir: boolean };
    sucursales: { leer: boolean; escribir: boolean };
    inventarios: { leer: boolean; escribir: boolean };
  };
}

export interface Sucursal {
  id: string;
  nombre: string;
  direccion: string;
  ciudad: string;
  telefono: string;
  gerente: string;
  activa: boolean;
}

export interface Producto {
  id: string;
  nombre: string;
  descripcion: string;
  noParte: string;
  precio: number;
  stock: number;
  minimo: number;
  sucursal: string;
  categoria: string;
}

export interface Movimiento {
  id: string;
  tipo: 'entrada' | 'salida' | 'transferencia' | 'merma';
  producto: string;
  cantidad: number;
  fecha: string;
  usuario: string;
  sucursal: string;
  origen?: string;
  destino?: string;
}

export const perfiles: Perfil[] = [
  {
    id: '1',
    nombre: 'Administrador',
    descripcion: 'Acceso completo al sistema',
    permisos: {
      usuarios: { leer: true, escribir: true },
      perfiles: { leer: true, escribir: true },
      sucursales: { leer: true, escribir: true },
      inventarios: { leer: true, escribir: true },
    },
  },
  {
    id: '2',
    nombre: 'Gerente de Sucursal',
    descripcion: 'Gestión de inventario y usuarios de sucursal',
    permisos: {
      usuarios: { leer: true, escribir: false },
      perfiles: { leer: true, escribir: false },
      sucursales: { leer: true, escribir: false },
      inventarios: { leer: true, escribir: true },
    },
  },
  {
    id: '3',
    nombre: 'Vendedor',
    descripcion: 'Solo consulta de inventario',
    permisos: {
      usuarios: { leer: false, escribir: false },
      perfiles: { leer: false, escribir: false },
      sucursales: { leer: false, escribir: false },
      inventarios: { leer: true, escribir: false },
    },
  },
];

export const sucursales: Sucursal[] = [
  {
    id: '1',
    nombre: 'Sucursal Centro',
    direccion: 'Av. Juárez 123',
    ciudad: 'Ciudad de México',
    telefono: '55-1234-5678',
    gerente: 'María García',
    activa: true,
  },
  {
    id: '2',
    nombre: 'Sucursal Norte',
    direccion: 'Blvd. Norte 456',
    ciudad: 'Monterrey',
    telefono: '81-9876-5432',
    gerente: 'Carlos Rodríguez',
    activa: true,
  },
  {
    id: '3',
    nombre: 'Sucursal Sur',
    direccion: 'Calle Sur 789',
    ciudad: 'Guadalajara',
    telefono: '33-5555-6666',
    gerente: 'Ana Martínez',
    activa: true,
  },
];

export const usuarios: Usuario[] = [
  {
    id: '1',
    nombre: 'Admin Sistema',
    email: 'admin@inventario.com',
    password: 'admin123',
    rol: 'Administrador',
    sucursal: 'Sucursal Centro',
    activo: true,
    fechaCreacion: '2024-01-15',
  },
  {
    id: '2',
    nombre: 'María García',
    email: 'maria@inventario.com',
    password: 'maria123',
    rol: 'Gerente de Sucursal',
    sucursal: 'Sucursal Centro',
    activo: true,
    fechaCreacion: '2024-02-01',
  },
  {
    id: '3',
    nombre: 'Carlos Rodríguez',
    email: 'carlos@inventario.com',
    password: 'carlos123',
    rol: 'Gerente de Sucursal',
    sucursal: 'Sucursal Norte',
    activo: true,
    fechaCreacion: '2024-02-01',
  },
  {
    id: '4',
    nombre: 'Ana Martínez',
    email: 'ana@inventario.com',
    password: 'ana123',
    rol: 'Gerente de Sucursal',
    sucursal: 'Sucursal Sur',
    activo: true,
    fechaCreacion: '2024-02-01',
  },
  {
    id: '5',
    nombre: 'Juan Pérez',
    email: 'juan@inventario.com',
    password: 'juan123',
    rol: 'Vendedor',
    sucursal: 'Sucursal Centro',
    activo: true,
    fechaCreacion: '2024-03-01',
  },
];

export const productos: Producto[] = [
  {
    id: '1',
    nombre: 'Laptop Dell Inspiron',
    descripcion: 'Laptop 15.6", Intel i5, 8GB RAM',
    noParte: 'DELL-INS-001',
    precio: 12500,
    stock: 15,
    minimo: 5,
    sucursal: 'Sucursal Centro',
    categoria: 'Electrónica',
  },
  {
    id: '2',
    nombre: 'Mouse Logitech M185',
    descripcion: 'Mouse inalámbrico',
    noParte: 'LOG-M185',
    precio: 250,
    stock: 3,
    minimo: 10,
    sucursal: 'Sucursal Centro',
    categoria: 'Accesorios',
  },
  {
    id: '3',
    nombre: 'Teclado Mecánico RGB',
    descripcion: 'Teclado mecánico con retroiluminación',
    noParte: 'KBD-MECH-001',
    precio: 1200,
    stock: 8,
    minimo: 5,
    sucursal: 'Sucursal Norte',
    categoria: 'Accesorios',
  },
  {
    id: '4',
    nombre: 'Monitor LG 24"',
    descripcion: 'Monitor Full HD IPS',
    noParte: 'LG-MON-24',
    precio: 3500,
    stock: 12,
    minimo: 8,
    sucursal: 'Sucursal Norte',
    categoria: 'Electrónica',
  },
  {
    id: '5',
    nombre: 'Cable HDMI 2.0',
    descripcion: 'Cable HDMI 2 metros',
    noParte: 'HDMI-2M',
    precio: 150,
    stock: 2,
    minimo: 15,
    sucursal: 'Sucursal Sur',
    categoria: 'Cables',
  },
  {
    id: '6',
    nombre: 'Webcam Logitech C920',
    descripcion: 'Cámara web Full HD 1080p',
    noParte: 'LOG-C920',
    precio: 1800,
    stock: 6,
    minimo: 5,
    sucursal: 'Sucursal Sur',
    categoria: 'Accesorios',
  },
];

export const movimientos: Movimiento[] = [
  {
    id: '1',
    tipo: 'entrada',
    producto: 'Laptop Dell Inspiron',
    cantidad: 10,
    fecha: '2026-03-05',
    usuario: 'María García',
    sucursal: 'Sucursal Centro',
  },
  {
    id: '2',
    tipo: 'salida',
    producto: 'Mouse Logitech M185',
    cantidad: 5,
    fecha: '2026-03-06',
    usuario: 'Juan Pérez',
    sucursal: 'Sucursal Centro',
  },
  {
    id: '3',
    tipo: 'transferencia',
    producto: 'Teclado Mecánico RGB',
    cantidad: 3,
    fecha: '2026-03-07',
    usuario: 'Carlos Rodríguez',
    sucursal: 'Sucursal Norte',
    origen: 'Sucursal Norte',
    destino: 'Sucursal Centro',
  },
  {
    id: '4',
    tipo: 'merma',
    producto: 'Cable HDMI 2.0',
    cantidad: 2,
    fecha: '2026-03-08',
    usuario: 'Ana Martínez',
    sucursal: 'Sucursal Sur',
  },
  {
    id: '5',
    tipo: 'entrada',
    producto: 'Monitor LG 24"',
    cantidad: 8,
    fecha: '2026-03-08',
    usuario: 'Carlos Rodríguez',
    sucursal: 'Sucursal Norte',
  },
];

// Datos para gráficas del dashboard
export const ventasComprasData = [
  { mes: 'Ene', ventas: 45000, compras: 32000 },
  { mes: 'Feb', ventas: 52000, compras: 38000 },
  { mes: 'Mar', ventas: 48000, compras: 35000 },
  { mes: 'Abr', ventas: 61000, compras: 42000 },
  { mes: 'May', ventas: 55000, compras: 39000 },
  { mes: 'Jun', ventas: 67000, compras: 45000 },
];

export const distribucionVentas = [
  { categoria: 'Electrónica', valor: 45000 },
  { categoria: 'Accesorios', valor: 28000 },
  { categoria: 'Cables', valor: 12000 },
  { categoria: 'Otros', valor: 15000 },
];
