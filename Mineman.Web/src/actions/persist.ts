export function store<T>(key: string, value: T){
    localStorage.setItem(key, JSON.stringify(value));
}

export function load<T>(key: string){
    const value = localStorage.getItem(key);

    return value  
        ? JSON.parse(value) as T
        : null;
}

export const clear = (key: string) =>
    localStorage.removeItem(key);

export const getPageUniqueStorageKey = (prefix: string) =>
    `${prefix}_${window.location.hash}`;