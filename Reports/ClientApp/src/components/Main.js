import React from 'react';
import PopupWithForm from './PopupWithForm';
import File from './File';
import {CurrentUserContext} from '../contexts/CurrentUserContext';
import {api} from '../utils/Api';

export default function Main({ files }) {
  const [isAddFilePopupOpen, setIsAddFilePopupOpen] = React.useState(false);
  const [isCreateReportPopupOpen, setIsCreateReportPopupOpen] = React.useState(false);
  const [file, setFile] = React.useState({});
  const currentUser = React.useContext(CurrentUserContext);
  function closeAllPopups() {
    setIsAddFilePopupOpen(false);
    setIsCreateReportPopupOpen(false);
  }

  function handleAddButtonClick() {
    setIsAddFilePopupOpen(true);
  }

  function handleUploadFile(file) {
    setFile(file);
  }

  function sendFile(e) {
    e.preventDefault();
    let formData = new FormData();
    formData.append('upload', file, file.name);
    api
      .uploadFile(currentUser.login, formData)
      .catch(error => console.log(error));
  }

  React.useEffect(() => console.log(files), []);

  return (
    <main className="content">
      <div className="content__header">
        <h2 className="content__title">Файлы с начислениями</h2>
        <button className="content__button" onClick={handleAddButtonClick}> Загрузить файл</button>
      </div>

      <ul className="files">
        {
          files && files.map(file => (
            <File 
              key={file.id}
              name={file.name}
              size={file.size}
              dateCreated={file.dateCreated}
            />
          ))
        }
      </ul>

      <PopupWithForm name="" buttonText="Добавить" isOpen={isAddFilePopupOpen} onClose={closeAllPopups}>
        <input type="file" onChange={e => handleUploadFile(e.target.files[0])} />
        <button onClick={(e) => sendFile(e)}>Отправить</button>
      </PopupWithForm>

      <PopupWithForm name="" buttonText="Создать" title="Создать отчёт" isOpen={isCreateReportPopupOpen} onClose={closeAllPopups}>
        <input className="form__input" type="text" placeholder="Название" required />
        <select className="form__select">
          <option value="pdf">PDF</option>
          <option value="excel">Excel</option>
        </select>
      </PopupWithForm>
    </main>
  );
}