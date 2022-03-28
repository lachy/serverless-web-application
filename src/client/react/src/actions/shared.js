export const API_ERROR = 'API_ERROR';

export function displayError(err) {
  return {
    type: API_ERROR,
    payload: err,
  };
}

export function clearError() {
  return {
    type: API_ERROR,
    payload: null,
  };
}
