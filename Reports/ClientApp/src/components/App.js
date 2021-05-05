import logo from './logo.svg';
import './App.css';

function App() {
  return (
    <>
    <Header isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />

    <Footer />
    </>
  );
}

export default App;
