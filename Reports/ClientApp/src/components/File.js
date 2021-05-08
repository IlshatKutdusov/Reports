export default function File(props) {
  return (
    <div className="file">
      <div className="file__text">
        <h3 className="file__name">{props.name}</h3>
        <p className="file__info">{props.info}</p>
      </div>   
      <div className="file__button-container">
        <button className="file__report-button" onClick={props.onReportCreate}></button>
        <button className="file__remove-button"></button>
      </div>
    </div>
  );
}