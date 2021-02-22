import { AxiosInstance } from "axios";
import MockAdapter from 'axios-mock-adapter';
import setupImagesApiMock from "./images";
import setupModsApiMock from "./mods";
import setupServerApiMock from "./server";
import setupWorldsApiMock from "./worlds";

const configureApiMock = (axios: AxiosInstance) => {
    const mockAdapter = new MockAdapter(axios, {delayResponse: 200});
    setupServerApiMock(mockAdapter);
    setupImagesApiMock(mockAdapter);
    setupWorldsApiMock(mockAdapter);
    setupModsApiMock(mockAdapter);
}

export default configureApiMock;