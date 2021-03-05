import { Message } from "../models/message";
import { Action } from "../reducer";

export const addMessage = (dispatch: React.Dispatch<Action>, message: Message) => {
    dispatch({type: 'messageAdded', message});
} 

export const clearMessage = (dispatch: React.Dispatch<Action>, message: Message) => {
    dispatch({type: 'messageCleared', message});
} 