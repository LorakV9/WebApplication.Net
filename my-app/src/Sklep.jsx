import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';

const Sklep = () => {
    const [products, setProducts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [userId, setUserId] = useState(null);
    const [cartItems, setCartItems] = useState([]); // Koszyk
    const [totalPrice, setTotalPrice] = useState(0); // Całkowita cena produktów w koszyku

    // Pobieranie produktów
    const fetchProducts = useCallback(async () => {
        let url = 'http://localhost:5126/api/products'; // Domyślnie pobieramy wszystkie produkty

        if (selectedCategory) {
            url = `http://localhost:5126/api/products/category/${selectedCategory}`; // Produkty po wybranej kategorii
        }

        try {
            const response = await axios.get(url);
            setProducts(response.data); // Ustawiamy produkty
        } catch (error) {
            console.error('Błąd podczas pobierania produktów:', error);
        }
    }, [selectedCategory]);  // Zależność: zmiana kategorii

    // Pobieranie kategorii
    const fetchCategories = useCallback(async () => {
        try {
            const response = await axios.get('http://localhost:5126/api/categories');
            setCategories(response.data); // Ustawiamy kategorie
        } catch (error) {
            console.error('Błąd podczas pobierania kategorii:', error);
        }
    }, []);  // Tylko raz, przy pierwszym renderze

    // Pobieranie produktów w koszyku
    const fetchCartItems = useCallback(async () => {
      if (!userId) return;
  
      try {
          const response = await axios.get(`http://localhost:5126/api/cart/${userId}`);
          console.log('Odpowiedź z serwera (koszyk):', response.data); // Logujemy odpowiedź z serwera
          setCartItems(response.data); // Ustawiamy produkty w koszyku
          calculateTotalPrice(response.data); // Obliczanie całkowitej ceny
      } catch (error) {
          console.error('Błąd podczas pobierania produktów w koszyku:', error);
      }
  }, [userId]);  // Zależność: zmiana userId  // Zależność: zmiana userId

    // Dodawanie produktu do koszyka
    const addToCart = async (productId) => {
        if (!userId) {
            alert('Musisz być zalogowany, aby dodać produkt do koszyka!');
            return;
        }

        const cartItem = {
            ProductId: productId,
            UserId: userId,
            Amount: 1, // Zakładamy, że dodajemy 1 sztukę produktu
        };

        try {
            await axios.post('http://localhost:5126/api/cart', cartItem);
            alert('Produkt został dodany do koszyka!');
            fetchCartItems(); // Odświeżamy koszyk po dodaniu
        } catch (error) {
            console.error('Błąd podczas dodawania produktu do koszyka:', error);
        }
    };

    // Obliczanie całkowitej ceny produktów w koszyku
    const calculateTotalPrice = (cartItems) => {
        let total = 0;
        cartItems.forEach(item => {
            total += item.price * item.amount; // Zakładamy, że mamy cenę i ilość w obiekcie cartItem
        });
        setTotalPrice(total);
    };

    useEffect(() => {
        fetchCategories(); // Pobieranie kategorii przy pierwszym renderze
    }, [fetchCategories]);

    useEffect(() => {
        fetchProducts(); // Pobieranie produktów przy zmianie kategorii
    }, [fetchProducts]);

    useEffect(() => {
      // Sprawdzamy, czy w localStorage mamy zapisany userId
      const storedUserId = localStorage.getItem('userId');
      if (storedUserId) {
          setUserId(storedUserId); // Ustawiamy ID użytkownika
          fetchCartItems(); // Pobieramy produkty w koszyku po zalogowaniu
      }
  }, []);  // Uruchomimy tylko raz, po pierwszym załadowaniu komponentu
  

    return (
        <div style={{ display: 'flex' }}>
            {/* Główna część sklepu */}
            <div style={{ flex: 1 }}>
                <h2>Produkty</h2>
                {/* Wybór kategorii */}
                <select
                    value={selectedCategory}
                    onChange={(e) => setSelectedCategory(e.target.value)}
                >
                    <option value="">Wybierz kategorię</option>
                    {categories.map((category) => (
                        <option key={category.id} value={category.id}>
                            {category.name}
                        </option>
                    ))}
                </select>

                {/* Produkty */}
                <div>
                    {products.length === 0 ? (
                        <p>Brak produktów w tej kategorii.</p>
                    ) : (
                        <ul>
                            {products.map((product) => (
                                <li key={product.id}>
                                    <h3>{product.name}</h3>
                                    <p>{product.price} PLN</p>
                                    <button onClick={() => addToCart(product.id)}>
                                        Dodaj do koszyka
                                    </button>
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
            </div>

            {/* Koszyk po prawej stronie */}
            <div style={{ width: '300px', marginLeft: '20px', border: '1px solid #ddd', padding: '10px' }}>
                <h3>Koszyk</h3>
                <ul>
                    {cartItems.length === 0 ? (
                        <p>Koszyk jest pusty.</p>
                    ) : (
                        cartItems.map(item => (
                            <li key={item.id}>
                                {item.name} - {item.amount} szt. - {item.price} PLN
                            </li>
                        ))
                    )}
                </ul>
                <hr />
                <p><strong>Łączna cena: {totalPrice} PLN</strong></p>
            </div>
        </div>
    );
};

export default Sklep;
