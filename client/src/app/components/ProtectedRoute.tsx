import { Navigate } from 'react-router';
import { useAuth } from '../context/AuthContext';
import { hasAnyPermission, type PermissionRequirement } from '../utils/permissions';

export function ProtectedRoute({
  children,
  requiredAny,
}: {
  children: React.ReactNode;
  requiredAny?: PermissionRequirement[];
}) {
  const { isAuthenticated, user } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requiredAny && requiredAny.length > 0 && !hasAnyPermission(user, requiredAny)) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}
