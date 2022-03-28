import { Middleware } from 'redux';

const logger: Middleware = (store) => (next) => (action) => {
  // eslint-disable-next-line no-console
  console.group(action.type);
  // eslint-disable-next-line no-console
  console.log('The action:', action);
  const returnValue = next(action);
  // eslint-disable-next-line no-console
  console.log('The new state:', store.getState());
  // eslint-disable-next-line no-console
  console.groupEnd();
  return returnValue;
};

export default logger;
