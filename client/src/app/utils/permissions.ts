import type { LoginResponseDto } from '../types';

export type PermisoAccion = 'leer' | 'escribir' | 'eliminar';

export interface PermissionRequirement {
  modulo: string;
  categoria: string;
  accion: PermisoAccion;
}

const normalize = (value?: string) => (value ?? '').trim().toLowerCase();

export function hasPermission(
  user: LoginResponseDto | null | undefined,
  requirement: PermissionRequirement
): boolean {
  if (!user) return false;

  const moduloNorm = normalize(requirement.modulo);
  const categoriaNorm = normalize(requirement.categoria);

  return user.permisos.some((p) => {
    const sameModulo = normalize(p.nombreModulo) === moduloNorm;
    const sameCategoria = normalize(p.nombreCategoria) === categoriaNorm;

    if (!sameModulo || !sameCategoria) return false;

    if (requirement.accion === 'leer') return p.puedeLeer;
    if (requirement.accion === 'escribir') return p.puedeEscribir;
    return p.puedeEliminar;
  });
}

export function hasAnyPermission(
  user: LoginResponseDto | null | undefined,
  requirements: PermissionRequirement[]
): boolean {
  return requirements.some((r) => hasPermission(user, r));
}
