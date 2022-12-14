import React from "react";
import "./ConfirmDialog.scss"
const ConfirmDialog = ({ cancelFunc, acceptFunc}) => {
  return (
    <section className="ConfirmDialog">
      <header>
        <h1>Esta seguro de eliminar la Blockchain?</h1>
        <h3 style={{color:"red"}}>ESTA OPERACION ES IRREVERSIBLE</h3>
      </header>
      <ul className="options-list">
        <li className="option"><button onClick={() => {
          cancelFunc()
        }}> <p>CANCELAR</p> </button></li>
        <li className="option"><button onClick={() => {
          acceptFunc()
        }}> <p>CONFIRMAR</p> </button></li>
      </ul>
    </section>
  );
};
export default ConfirmDialog;