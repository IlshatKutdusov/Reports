import React from 'react';
import PopupWithForm from './PopupWithForm';

export default function Main() {
  const [isAddFilePopupOpen, setIsAddFilePopupOpen] = React.useState(false);
  const [isCreateReportPopupOpen, setIsCreateReportPopupOpen] = React.useState(false);

  function closeAllPopups() {
    setIsAddFilePopupOpen(false);
    setIsCreateReportPopupOpen(false);
  }

  function handleAddButtonClick() {
    setIsAddFilePopupOpen(true);
  }

  return (
    <main className="content">
      <div className="content__header">
        <h2 className="content__title">Файлы с начислениями</h2>
        <button className="content__button" onClick={handleAddButtonClick} > Загрузить файл</button>
      </div>
      <PopupWithForm name="" buttonText="Добавить" isOpen={isAddFilePopupOpen} onClose={closeAllPopups}>
        <input type="file" />
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