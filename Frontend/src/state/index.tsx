import React, { createContext, useContext, useReducer } from 'react';
import { ApplicationState } from './initial';
import { Action } from '../reducer/index';

interface StateContextReturnType {
    state: ApplicationState,
    dispatch: React.Dispatch<Action>
}

interface StateProviderProperties {
    reducer: any;
    initialState: ApplicationState;
}

export const StateContext = createContext({});

export const StateProvider: React.FC<StateProviderProperties> = ({ reducer, initialState, children }) => {
    const [state, dispatch] = useReducer(reducer, initialState);
    const value = {state, dispatch};

    return (
        <StateContext.Provider value={value}>
            { children }
        </StateContext.Provider>
  );
};

// eslint-disable-next-line react-hooks/rules-of-hooks
export const getState = () => useContext(StateContext) as StateContextReturnType;