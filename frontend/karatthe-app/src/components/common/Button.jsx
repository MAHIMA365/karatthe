const Button = ({ children, type = 'button', disabled = false, onClick, className = '' }) => {
  return (
    <button
      type={type}
      disabled={disabled}
      onClick={onClick}
      className={`btn ${className}`}
      style={{
        padding: '1rem 2rem',
        background: disabled ? '#ccc' : 'linear-gradient(135deg, #ff6b35 0%, #f7931e 100%)',
        color: 'white',
        border: 'none',
        borderRadius: '25px',
        cursor: disabled ? 'not-allowed' : 'pointer',
        fontSize: '1rem',
        fontWeight: '600',
        transition: 'all 0.3s ease',
        boxShadow: disabled ? 'none' : '0 4px 15px rgba(255, 107, 53, 0.4)'
      }}
    >
      {children}
    </button>
  );
};

export default Button;