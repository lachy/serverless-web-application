import * as React from 'react';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import Link from '@mui/material/Link';
import { useAuth0 } from '@auth0/auth0-react';
import { Button, CircularProgress } from '@mui/material';
import ButtonAppBar from './components/AppBar';
import Profile from './components/Profile';
import useTodosApi from './api/useTodoApi';

function Copyright() {
  return (
    <Typography variant="body2" color="text.secondary" align="center">
      {'Copyright © '}
      <Link color="inherit" href="https://mui.com/">
        Your Website
      </Link>
      {' '}
      {new Date().getFullYear()}
      .
    </Typography>
  );
}

export default function App() {
  const { isLoading } = useAuth0();
  const { getAllTodos } = useTodosApi();

  const handleButtonClick = async () => {
    await getAllTodos();
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex' }}>
        <CircularProgress />
      </Box>
    );
  }
  return (
    <Container>
      <Box>
        <ButtonAppBar />
        <Profile />
        <Button onClick={handleButtonClick}>
          Make Backend Api Call
        </Button>
        <Copyright />
      </Box>
    </Container>
  );
}
