import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';

const Sklep = () => {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [selectedCategory, setSelectedCategory] = useState('');
  const [userId, setUserId] = useState(null);
  const [userName, setUserName] = useState('');
  const [cartItems, setCartItems] = useState([]);
  const [totalPrice, setTotalPrice] = useState(0);
  const [isAdding, setIsAdding] = useState(false); // Dodany stan dla loadera

  // Pobieranie produktów
  const fetchProducts = useCallback(async () => {
    let url = 'http://localhost:5126/api/products';

    if (selectedCategory) {
      url = `http://localhost:5126/api/products/category/${selectedCategory}`;
    }

    try {
      const response = await axios.get(url);
      setProducts(response.data);
    } catch (error) {
      console.error('Błąd podczas pobierania produktów:', error);
    }
  }, [selectedCategory]);

  // Pobieranie kategorii
  const fetchCategories = useCallback(async () => {
    try {
      const response = await axios.get('http://localhost:5126/api/categories');
      setCategories(response.data);
    } catch (error) {
      console.error('Błąd podczas pobierania kategorii:', error);
    }
  }, []);

  // Pobieranie produktów w koszyku
  const fetchCartItems = useCallback(async () => {
    if (!userId) return;

    try {
      const response = await axios.get(`http://localhost:5126/api/cart/cart/${userId}`);
      if (Array.isArray(response.data)) {
        const mappedCartItems = response.data.map((item) => ({
          id: item.id,
          name: item.name,
          amount: item.amount,
          price: item.price,
        }));
        setCartItems(mappedCartItems);
        calculateTotalPrice(mappedCartItems);
      } else {
        setCartItems([]);
      }
    } catch (error) {
      console.error('Błąd podczas pobierania produktów w koszyku:', error);
    }
  }, [userId]);

  // Obliczanie całkowitej ceny produktów w koszyku
  const calculateTotalPrice = (cartItems) => {
    const total = cartItems.reduce((sum, item) => sum + item.price * item.amount, 0);
    setTotalPrice(total);
  };

  // Dodawanie produktu do koszyka
  const addToCart = async (productId, productName, productPrice) => {
    // Sprawdzamy, czy użytkownik jest zalogowany
    if (!userId) {
      alert('Musisz być zalogowany, aby dodać produkt do koszyka!');
      return;
    }
  
    // Tworzymy obiekt cartItem z poprawnymi danymi
    const cartItem = {
      ProductId: productId, 
      Name: productName,   
      Price: productPrice,   // Id produktu
        // Id zalogowanego użytkownika
      Amount: 1,   
      UserId: parseInt(userId, 10),             // Ilość dodawanego produktu (na razie 1)
            // Nazwa produktu
           // Cena produktu
    };
  
    try {
      // Wysłanie danych do backendu
      const response = await axios.post('http://localhost:5126/api/cart', cartItem, {
        headers: { 'Content-Type': 'application/json' },
      });
  
      console.log('Odpowiedź z serwera (dodanie do koszyka):', response.data);
      alert('Produkt został dodany do koszyka!');
      
      // Odświeżamy koszyk po dodaniu produktu
      fetchCartItems(); 
    } catch (error) {
      console.error('Błąd podczas dodawania produktu do koszyka:', error);
      if (error.response) {
        console.error('Szczegóły odpowiedzi błędu:', error.response.data);
      }
      alert('Nie udało się dodać produktu do koszyka.');
    }
  };
  
  
  

  useEffect(() => {
    fetchCategories();
  }, [fetchCategories]);

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]);

  useEffect(() => {
    const storedUserId = localStorage.getItem('userId');
    const storedUserName = localStorage.getItem('userName');
    if (storedUserId) {
      setUserId(storedUserId);
      setUserName(storedUserName);
    }
  }, []);

  useEffect(() => {
    if (userId) {
      fetchCartItems();
    }
  }, [userId, fetchCartItems]);

  return (
    <div style={{ display: 'flex' }}>
      <div style={{ flex: 1 }}>
        <h2>Produkty</h2>
        {userId && userName && (
          <div>
            <h3>Witaj, {userName}!</h3>
            <p>ID użytkownika: {userId}</p>
          </div>
        )}
        <select value={selectedCategory} onChange={(e) => setSelectedCategory(e.target.value)}>
          <option value="">Wybierz kategorię</option>
          {categories.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </select>

        <div>
          {products.length === 0 ? (
            <p>Brak produktów w tej kategorii.</p>
          ) : (
            <ul>
              {products.map((product) => (
                <li key={product.id}>
                  <h3>{product.name}</h3>
                  <p>{product.price} PLN</p>
                  <button onClick={() => addToCart(product.id, product.name, product.price)} disabled={isAdding}>
                    {isAdding ? 'Dodawanie...' : 'Dodaj do koszyka'}
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>

      <div style={{ width: '300px', marginLeft: '20px', border: '1px solid #ddd', padding: '10px' }}>
        <h3>Koszyk</h3>
        <ul>
          {cartItems.length === 0 ? (
            <p>Koszyk jest pusty.</p>
          ) : (
            cartItems.map((item) => (
              <li key={item.id}>
                {item.name} - {item.amount} szt. - {item.price.toFixed(2)} PLN
              </li>
            ))
          )}
        </ul>
        <hr />
        <p><strong>Łączna cena: {totalPrice.toFixed(2)} PLN</strong></p>
      </div>
    </div>
  );
};

export default Sklep;
