import { render, screen } from '@testing-library/react';
import userEvent, { fireEvent, click } from '@testing-library/user-event';
import { SymbolGrid } from "./symbolGrid";

describe('SymbolGrid', () => {
  it('should render the SymbolInput component', () => {
    const mockAddSymbol = jest.fn();
    const mockRemoveSymbol = jest.fn();
    const symbols = [];

    render(<SymbolGrid addSymbol={mockAddSymbol} removeSymbol={mockRemoveSymbol} symbols={symbols} />);

    expect(screen.getByRole('textbox')).toBeInTheDocument(); // Check for input element
  });

  it('should render the grid header and rows', () => {
    const mockAddSymbol = jest.fn();
    const mockRemoveSymbol = jest.fn();
    const symbols = [
      { name: 'Symbol 1', value: 10.0 },
      { name: 'Symbol 2', value: 20.5 },
    ];
  
    render(<SymbolGrid addSymbol={mockAddSymbol} removeSymbol={mockRemoveSymbol} symbols={symbols} />);
  
    // Check for grid header elements
    expect(screen.getByText('Symbol')).toBeInTheDocument();
    expect(screen.getByText('Price')).toBeInTheDocument();
    expect(screen.getByText('Actions')).toBeInTheDocument();
  
    // // Check for each symbol row
    symbols.forEach((symbol) => {
      expect(screen.getByText(symbol.name)).toBeInTheDocument();
      expect(screen.getByText(symbol.value.toFixed(2))).toBeInTheDocument();
    });
  });

  it('should call addSymbol on input submit', () => {
    const mockAddSymbol = jest.fn();
    const mockRemoveSymbol = jest.fn();
    const symbols = [];
  
    render(<SymbolGrid addSymbol={mockAddSymbol} removeSymbol={mockRemoveSymbol} symbols={symbols} />);
  
    userEvent.type(screen.getByRole('textbox'), 'New Symbol');
    const addButton = screen.getByText('Add Symbol');
    userEvent.click(addButton);
  
    expect(mockAddSymbol).toHaveBeenCalledWith('New Symbol');
  });

  it('should call removeSymbol on button click', () => {
    const mockAddSymbol = jest.fn();
    const mockRemoveSymbol = jest.fn();
    const symbols = [{ name: 'Symbol 1', value: 10.0 }];
  
    render(<SymbolGrid addSymbol={mockAddSymbol} removeSymbol={mockRemoveSymbol} symbols={symbols} />);
  
    const closeButton = screen.getByText('Close');
    userEvent.click(closeButton);
  
    expect(mockRemoveSymbol).toHaveBeenCalledWith('Symbol 1');
  });
});
