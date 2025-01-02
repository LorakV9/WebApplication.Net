import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const Register = ({ onRegisterSuccess }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [error, setError] = useState(null);
    const navigate = useNavigate(); // Hook do nawigacji

    const handleRegister = async (e) => {
        e.preventDefault();

        try {
            const response = await axios.post('http://localhost:5126/api/users', {
              imie: firstName,
              nazwisko: lastName, 
              Email: email,
              Password: password
            });

            console.log(response.data); // Sprawdź odpowiedź z serwera
            alert('Rejestracja zakończona pomyślnie!');
            onRegisterSuccess(); // Ustawienie stanu logowania
            navigate('/'); // Przekierowanie na stronę główną (Sklep)
        } catch (err) {
            setError(err.response ? err.response.data : 'Błąd połączenia z serwerem');
        }
    };

    return (
        <div>
            <form onSubmit={handleRegister}>
                <input 
                    type="text" 
                    value={firstName} 
                    onChange={(e) => setFirstName(e.target.value)} 
                    placeholder="Imię" 
                    required 
                />
                <input 
                    type="text" 
                    value={lastName} 
                    onChange={(e) => setLastName(e.target.value)} 
                    placeholder="Nazwisko" 
                    required 
                />
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
                <button type="submit">Zarejestruj się</button>
            </form>
            {error && <p style={{color: 'red'}}>{error}</p>}
        </div>
    );
};

export default Register;
