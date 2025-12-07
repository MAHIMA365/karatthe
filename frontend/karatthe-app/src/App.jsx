import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/Authcontext.jsx';
import AuthLayout from './components/auth/Authlayout';
import AdminDashboard from './components/admin/AdminDashboard';
import ProtectedRoute from './components/common/ProtectedRoute';
import './App.css';

const Dashboard = () => {
  const { user, logout, isAuthenticated } = useAuth();
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  return (
    <div className="dashboard">
      <header>
        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
          <CartIcon size={32} />
          <h1 className="sinhala-title">කරත්තේ - Welcome, {user?.email}</h1>
        </div>
        <button onClick={logout}>Logout</button>
      </header>
      
      {user?.role === 'Admin' && (
        <div className="admin-section">
          <h3>Admin Panel</h3>
          <AdminDashboard />
        </div>
      )}
      
      <div className="user-content">
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', gap: '2rem', marginBottom: '2rem' }}>
          <div style={{ background: 'rgba(255, 255, 255, 0.9)', padding: '2rem', borderRadius: '15px', textAlign: 'center', boxShadow: '0 5px 15px rgba(0,0,0,0.1)' }}>
            <CartIcon size={40} />
            <h3 style={{ margin: '1rem 0', color: '#ff6b35' }}>My Cart</h3>
            <p style={{ color: '#666' }}>0 items</p>
          </div>
          <div style={{ background: 'rgba(255, 255, 255, 0.9)', padding: '2rem', borderRadius: '15px', textAlign: 'center', boxShadow: '0 5px 15px rgba(0,0,0,0.1)' }}>
            <h3 style={{ margin: '1rem 0', color: '#ff6b35' }}>Orders</h3>
            <p style={{ color: '#666' }}>0 orders</p>
          </div>
          <div style={{ background: 'rgba(255, 255, 255, 0.9)', padding: '2rem', borderRadius: '15px', textAlign: 'center', boxShadow: '0 5px 15px rgba(0,0,0,0.1)' }}>
            <h3 style={{ margin: '1rem 0', color: '#ff6b35' }}>Profile</h3>
            <p style={{ color: '#666' }}>Role: {user?.role}</p>
          </div>
        </div>
      </div>
    </div>
  );
};

const AppRoutes = () => {
  const { isAuthenticated } = useAuth();
  
  return (
    <Routes>
      <Route 
        path="/login" 
        element={isAuthenticated ? <Navigate to="/dashboard" replace /> : <AuthLayout />} 
      />
      <Route 
        path="/dashboard" 
        element={
          <ProtectedRoute>
            <Dashboard />
          </ProtectedRoute>
        } 
      />
      <Route 
        path="/admin" 
        element={
          <ProtectedRoute adminOnly>
            <AdminDashboard />
          </ProtectedRoute>
        } 
      />
      <Route 
        path="/" 
        element={<Navigate to={isAuthenticated ? "/dashboard" : "/login"} replace />} 
      />
    </Routes>
  );
};

function App() {
  return (
    <AuthProvider>
      <Router>
        <div className="app">
          <AppRoutes />
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App
