import React from 'react';

export default function SignInForm({ onSubmit }) {
  const [login, setLogin] = React.useState('');
  const [password, setPassword] = React.useState('');

  function handleLoginInput(event) {
    setLogin(event.target.value);
  }

  function handlePasswordInput(event) {
    setPassword(event.target.value);
  }

  function handleSubmit(event) {
    event.preventDefault();

    onSubmit({
      login: login,
      password: password,
    });
  }

  return (
    <form className="form" action="#" onSubmit={handleSubmit}>
      <input className="form__input" type="login" placeholder="Логин" required onChange={handleLoginInput} value={ login || '' }/>
      <input className="form__input" type="password" placeholder="Пароль" required onChange={handlePasswordInput} value = { password || '' }/>
      <div className="form__button-container">
        <button className="form__button form__button_type_login" type="submit">Войти</button>
        <a className="link link_type_forgot" href="#">Забыли пароль?</a>
      </div>
    </form>
  );
} 