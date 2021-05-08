import moment from 'moment';

export default function File(props) {
  function handleDelete() {
    props.onDelete(props.id);
  }

  function formatDate(dateString) {
    return moment(dateString).format('DD.MM.YYYY HH:mm');
  }

  return (
    <div className="file">
      <div className="file__text">
        <h3 className="file__name">{props.name.split('_')[1]}</h3>
        <p className="file__info">{`${formatDate(props.dateCreated)}, ${props.size} MB`}</p>
      </div>   
      <div className="file__button-container">
        <button className="file__report-button" onClick={props.onReportCreate}></button>
        <button className="file__remove-button" onClick={handleDelete}></button>
      </div>
    </div>
  );
}