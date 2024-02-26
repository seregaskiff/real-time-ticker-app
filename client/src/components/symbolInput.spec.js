import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { SymbolInput } from './symbolInput';

describe('SymbolInput', () => {
  it('should focus the input element on mount', () => {
    const mockAddSymbol = jest.fn();
    render(<SymbolInput addSymbol={mockAddSymbol} />);

    expect(document.activeElement).toBe(document.querySelector('input'));
  });

  it('should call addSymbol with trimmed value on submit', () => {
    const mockAddSymbol = jest.fn();
    render(<SymbolInput addSymbol={mockAddSymbol} />);

    userEvent.type(document.querySelector('input'), '  Some Symbol  ');
    const addButton = screen.getByText('Add Symbol');
    userEvent.click(addButton);

    expect(mockAddSymbol).toHaveBeenCalledWith('Some Symbol');
  });

  it('should clear the input and refocus on successful add', () => {
    const mockAddSymbol = jest.fn();
    render(<SymbolInput addSymbol={mockAddSymbol} />);

    userEvent.type(document.querySelector('input'), 'Another Symbol');
    const addButton = screen.getByText('Add Symbol');
    userEvent.click(addButton);

    expect(document.querySelector('input').value).toBe('');
    expect(document.activeElement).toBe(document.querySelector('input'));
  });

  it('should not call addSymbol with empty value', () => {
    const mockAddSymbol = jest.fn();
    render(<SymbolInput addSymbol={mockAddSymbol} />);

    const addButton = screen.getByText('Add Symbol');
    userEvent.click(addButton);

    expect(mockAddSymbol).not.toBeCalled();
  });

  it('should call addSymbol on Enter key press', () => {
    const mockAddSymbol = jest.fn();
    render(<SymbolInput addSymbol={mockAddSymbol} />);
    const input = document.querySelector('input');
    userEvent.type(input, 'Symbol with Enter');
    userEvent.keyboard("{enter}");
    expect(mockAddSymbol).toHaveBeenCalledWith('Symbol with Enter');
  });
});