import React from 'react';
import { Route, Switch, useHistory } from 'react-router';
import Header from './Header';
import Footer from './Footer';
import Lobby from './Lobby';
import ProtectedRoute from './ProtectedRoute';
import SignInForm from './SignInForm';
import SignUpForm from './SignUpForm';
import Main from './Main';
import { CurrentUserContext } from '../contexts/CurrentUserContext';
import { api } from '../utils/Api';

function App() {
  const [currentUser, setCurrentUser] = React.useState({});
  const [isLoggedIn, setIsLoggedIn] = React.useState(false);
  const [isLoading, setIsLoading] = React.useState(false);
  const history = useHistory();

  function handleSignIn(authData) {
    setIsLoading(true);
    api
      .authenticate(authData)
      .then(response => {
        localStorage.setItem('jwt', response.token);
        setCurrentUser({
          login: response.login,
        });
        setIsLoggedIn(true);
        history.push('/');
      })
      .catch(error => console.log(error))
      .finally(() => setIsLoading(false));
  }

  function handleSignUp(regData) {
    setIsLoading(true);
    api
      .register(regData)
      .then(response => {
        
      })
      .catch(error => console.log(error))
      .finally(() => setIsLoading(false));
  }

  return (
    <div className="page">
      <CurrentUserContext.Provider value={currentUser}>
        <Header isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />

        <Switch>
            <Route exact path="/signin">
              <Lobby>
                <SignInForm onSubmit={handleSignIn} />
              </Lobby>
            </Route>
            <Route exact path="/signup">
              <Lobby>
                <SignUpForm onSubmit={handleSignUp} />
              </Lobby>
            </Route>
          <ProtectedRoute component={Main} path="/" isLoggedIn={isLoggedIn}>
            </ProtectedRoute>
          </Switch>

        <Footer />
      </CurrentUserContext.Provider>
    </div>
  );
}

export default App;
