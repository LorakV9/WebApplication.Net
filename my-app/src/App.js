import React, { useState } from "react";
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';

import Register from './Register';
import Login from './Login';
import Sklep from './Sklep'; // Strona główna

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false); // Stan logowania użytkownika

  return (
    <BrowserRouter>
      <Routes>
        <Route
          path="/register"
          element={<Register onRegisterSuccess={() => setIsAuthenticated(true)} />}
        />
        <Route
          path="/login"
          element={<Login onLoginSuccess={() => setIsAuthenticated(true)} />}
        />
        {/* Przekierowanie na stronę główną (Sklep) po zalogowaniu */}
        <Route
          path="/"
          element={isAuthenticated ? <Sklep /> : <Navigate to="/login" />}
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
