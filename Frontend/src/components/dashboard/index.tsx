import styled from '@emotion/styled';
import React from 'react';
import { getState } from '../../state';
import ServerCard from './serverCard';

const DashboardContainer = styled.div`
  margin:5px;
  display: flex;
  flex-wrap: wrap;

  * {
    margin-right: 10px;
  }
`;

const Dashboard: React.FunctionComponent = () => {
  const { state: { servers } } = getState();

  return (
    <DashboardContainer>
      {servers.map(s => <ServerCard server={s} key={s.id} />)}
    </DashboardContainer>
  );
};

export default Dashboard;