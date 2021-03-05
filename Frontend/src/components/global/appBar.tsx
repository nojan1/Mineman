import styled from '@emotion/styled';
import React from 'react';
import Navbar from 'react-bootstrap/Navbar';

const StyledNavbar = styled(Navbar)`
    background-image: linear-gradient(90deg, rgba(0,133,66,0) 0%, rgba(0,133,66,1) 80%, rgba(0,133,66,1) 100%), url('images/bg-grass.png');
    background-repeat: no-repeat;
    background-size: 20%;
`;

const BrandContainer = styled.div`
    display: flex;
    margin: 5px auto;
    align-items: center;

    h1 {
        margin: 0 10px;
        padding: 0;
        font-size: 18pt;
    }

    img {
        height: 30px;
    }
`;

const CustomBrand = () =>
    <BrandContainer>
        <img src="images/grass.png" />
        <h1>Mineman</h1>
        <img src="images/grass.png" />
    </BrandContainer>

const AppBar:React.FunctionComponent = () => {

    return (
        <StyledNavbar bg='primary' variant='dark'>
            <CustomBrand />
        </StyledNavbar>
    );
};

export default AppBar;