import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const Login = ({ onLoginSuccess }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState(null);
    const navigate = useNavigate(); // Hook do nawigacji

    const handleLogin = async (e) => {
      e.preventDefault();
    
      try {
        const response = await axios.post('http://localhost:5126/api/users/login', {
          Email: email,
          Password: password,
        });
    
        console.log('Odpowiedź z serwera logowania:', response.data);
    
        if (response.data.id) {
          // Zapisz userId i inne dane użytkownika w localStorage
          localStorage.setItem('userId', response.data.id);
          localStorage.setItem('userName', response.data.name); // Zakładając, że response zawiera "name"
          console.log('userId i userName zapisano w localStorage:', response.data.id, response.data.name);
    
          alert('Logowanie zakończone pomyślnie!');
          onLoginSuccess(); // Ustawienie stanu logowania
          navigate('/'); // Przekierowanie na stronę główną (Sklep)
        } else {
          alert('Błąd logowania: Brak ID użytkownika');
        }
      } catch (err) {
        setError(err.response ? err.response.data : 'Błąd połączenia z serwerem');
      }
    };
  

    return (
        <div>
            <form onSubmit={handleLogin}>
                <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="Email"
                    required
                />
                <input
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    placeholder="Hasło"
                    required
                />
                <button type="submit">Zaloguj się</button>
            </form>
            {error && <p style={{ color: 'red' }}>{error}</p>}
        </div>
    );
};

export default Login;
