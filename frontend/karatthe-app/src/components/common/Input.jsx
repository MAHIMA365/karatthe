const Input = ({ type = 'text', name, placeholder, value, onChange, required = false, className = '' }) => {
  return (
    <input
      type={type}
      name={name}
      placeholder={placeholder}
      value={value}
      onChange={onChange}
      required={required}
      className={`input ${className}`}
      style={{
        padding: '1rem 1.5rem',
        border: '2px solid rgba(255, 107, 53, 0.2)',
        borderRadius: '15px',
        fontSize: '1rem',
        width: '100%',
        boxSizing: 'border-box',
        background: 'rgba(255, 255, 255, 0.9)',
        transition: 'all 0.3s ease',
        outline: 'none'
      }}
      onFocus={(e) => {
        e.target.style.borderColor = 'rgba(255, 107, 53, 0.2)';
      }}
      onBlur={(e) => {
        e.target.style.borderColor = 'rgba(102, 126, 234, 0.2)';
        e.target.style.boxShadow = 'none';
      }}
    />
  );
};

export default Input;