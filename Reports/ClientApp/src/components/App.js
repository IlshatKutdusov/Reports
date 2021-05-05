import React from 'react';
import Header from './Header';
import Footer from './Footer';
import Lobby from './Lobby';
import ProtectedRoute from './ProtectedRoute';
import { CurrentUserContext } from '../contexts/CurrentUserContext';

function App() {
  return (
    <CurrentUserContext.Provider value={currentUser}>
      <Header isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />

      <Switch>
        <Route exact path="/signin">
          <Lobby>
          </Lobby>
        </Route>
        <Route exact path="/signup">
          <Lobby>
          </Lobby>
        </Route>
        <ProtectedRoute component={Main} path="/" isLoggedIn={isLoggedIn}>
        </ProtectedRoute>
      </Switch>

      <Footer />
    </CurrentUserContext.Provider>
  );
}

export default App;
