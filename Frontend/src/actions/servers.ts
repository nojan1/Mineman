import { Action } from "../reducer";

export const create = (dispatch: React.Dispatch<Action>, x: any) => new Promise<void>((resolve,  reject) => {
    setTimeout(resolve, 2000);
});

export const update = (dispatch: React.Dispatch<Action>, x: any) => new Promise<void>((resolve,  reject) => {
    setTimeout(resolve, 2000);
});

export const remove = (dispatch: React.Dispatch<Action>, x: any) => new Promise<void>((resolve,  reject) => {
    setTimeout(resolve, 2000);
});

export const startServer = (dispatch: React.Dispatch<Action>, x: any) => new Promise<void>((resolve,  reject) => {
    setTimeout(resolve, 2000);
});

export const stopServer = (dispatch: React.Dispatch<Action>, x: any) => new Promise<void>((resolve,  reject) => {
    setTimeout(resolve, 2000);
});