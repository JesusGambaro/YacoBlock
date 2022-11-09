import React,{useState} from "react";
import "./DatePickerStyle.scss"
const DatePicker = ({ cancelFunc, acceptFunc}) => {
    const [date, setDate] = useState()
    const [error,setError] = useState()
    const verifyErrors = () => {
      if (!date) {
        setError("Este Campo es obligatorio");
      }else{
        setError();
        acceptFunc(date.toString());
      }
    }
    return (
    <section className="DatePicker">
      <header>
        <h1>Elige una fecha para el bloque</h1>
      </header>
      <div className="input">
        <input type="date" onChange={(e) => {
          setDate(e.target.value.split('-').reverse().join('/'));
          
          console.log(e.target.value.split('-').reverse().join('/'));
        }} />
        <h3 style={{color:"red"}}>{error}</h3>
      </div>
      <ul className="options-list">
        <li className="option"><button onClick={() => {
          verifyErrors()
        }}>Confirmar</button></li>
        <li className="option"><button onClick={() => {
          cancelFunc()
        }}>Cancelar</button></li>
      </ul>
    </section>
  );
};
export default DatePicker;