import React from 'react';
import Header from './Header';
import Footer from './Footer';
import { CurrentUserContext } from '../contexts/CurrentUserContext';

function App() {
  return (
    <CurrentUserContext.Provider value={currentUser}>
      <Header isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />

      <Footer />
    </CurrentUserContext.Provider>
  );
}

export default App;
