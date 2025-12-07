import { Navigate } from 'react-router-dom';
import { useAuth } from '../../context/Authcontext.jsx';

const ProtectedRoute = ({ children, adminOnly = false }) => {
  const { isAuthenticated, isAdmin, loading } = useAuth();

  if (loading) {
    return <div>Loading...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (adminOnly && !isAdmin) {
    return <div>Access denied. Admin privileges required.</div>;
  }

  return children;
};

export default ProtectedRoute;