import { AxiosInstance } from "axios";
import MockAdapter from 'axios-mock-adapter';
import setupServerApiMock from "./server";

const configureApiMock = (axios: AxiosInstance) => {
    const mockAdapter = new MockAdapter(axios, {delayResponse: 10});
    setupServerApiMock(mockAdapter);
}

export default configureApiMock;