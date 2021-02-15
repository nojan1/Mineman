import React from 'react';
import ReactDOM from 'react-dom';
import { StateProvider } from './state';
import { getInitialState } from './state/initial';
import { mainReducer } from './reducer';
import App from './app';
import reportWebVitals from './reportWebVitals';

import './styles/main.scss'

ReactDOM.render(
  <React.StrictMode>
    <StateProvider initialState={getInitialState()} reducer={mainReducer}>
      <App />
    </StateProvider>
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
