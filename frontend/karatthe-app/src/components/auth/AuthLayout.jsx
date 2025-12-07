import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import LoginForm from './Loginform';
import RegisterForm from './Registerform';
import CartIcon from '../common/CartIcon';

const AuthLayout = () => {
  const [isLogin, setIsLogin] = useState(true);
  const navigate = useNavigate();

  const handleAuthSuccess = () => {
    navigate('/dashboard');
  };

  return (
    <div className="auth-layout">
      <div className="auth-container">
        <div style={{ textAlign: 'center', marginBottom: '2rem' }}>
          <CartIcon size={48} />
          <h1 className="sinhala-title" style={{ margin: '1rem 0', color: '#ff6b35' }}>කරත්තේ</h1>
          <p style={{ color: '#666', fontSize: '0.9rem' }}>Your Shopping Cart Solution</p>
        </div>
        {isLogin ? (
          <LoginForm onSuccess={handleAuthSuccess} />
        ) : (
          <RegisterForm onSuccess={() => setIsLogin(true)} />
        )}
        
        <div className="auth-toggle">
          {isLogin ? (
            <p>
              Don't have an account?{' '}
              <button onClick={() => setIsLogin(false)}>Register</button>
            </p>
          ) : (
            <p>
              Already have an account?{' '}
              <button onClick={() => setIsLogin(true)}>Login</button>
            </p>
          )}
        </div>
      </div>
    </div>
  );
};

export default AuthLayout;