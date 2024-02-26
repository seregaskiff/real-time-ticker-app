import { render, screen } from '@testing-library/react';
import {LiveCell} from './liveCell';


describe('LiveCell', () => {
  it('should show the provided value', () => {
    const value = 12.34;
    render(<LiveCell value={value} />);

    expect(screen.getByText(value.toFixed(2))).toBeInTheDocument();
  });

  it('should always have the "value-up" class when value is greater', () => {
    const value = 12.34;
    render(<LiveCell value={value} />);
  
    const cell = screen.getByTestId("live-cell");
    expect(cell).toHaveClass('value-up');
  });

  it('should always have the "value-down" class when value is lower', () => {
    const value = -12.34;
    render(<LiveCell value={value} />);
  
    const cell = screen.getByTestId("live-cell");
    expect(cell).toHaveClass('value-down');
  });


  it('should not have the "pulsing" class when value is undefined', () => {
    render(<LiveCell value={undefined} />);
  
    const cell = screen.getByTestId("live-cell"); // Use queryByText for undefined values
    expect(cell).not.toHaveClass('pulsing');
  });

});