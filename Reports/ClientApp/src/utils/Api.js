import { getToken } from "./token";

class Api {
  constructor({baseUrl, headers}) {
    this._baseUrl = baseUrl;
    this._headers = headers;
  }

  authenticate(authData) {
    return fetch(`${this._baseUrl}/User/Login`, {
      method: 'POST',
      headers: this._headers,
      body: JSON.stringify({
        login: authData.login,
        password: authData.password,
      }),
    })
    .then(this._checkResponse);
  }

  register(regData) {
    return fetch(`${this._baseUrl}/User/Register`, {
      method: 'POST',
      headers: this._headers,
      body: JSON.stringify({
        email: regData.email,
        login: regData.login,
        name: regData.name,
        password: regData.password,
        surname: regData.surname,
      }),
    })
    .then(this._checkResponse);
  }

  getUserData(login) {
    return fetch(`${this._baseUrl}/User?userLogin=${login}`, {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${getToken()}`,
      }
    })
    .then(this._checkResponse);
  }

  uploadFile(login, formData) {
    return fetch(`${this._baseUrl}/File/Upload?userLogin=${login}`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${getToken()}`,
      },
      body: formData,
    })
    .then(this._checkResponse);
  }

  _checkResponse(res) {
    if (res.ok) {
      return res.json();
    }
    
    return Promise.reject(`Ошибка: ${res.status}`); 
  }
}

export const api = new Api({
  baseUrl: 'https://localhost:44366/api',
  headers: {
    'Content-Type': 'application/json',
  }
})