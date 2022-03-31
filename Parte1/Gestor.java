package Parte1;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;

public class Gestor {

    BaseDeDatos bd = new BaseDeDatos();
    Connection conexion = bd.estableceConexion();

    public void cerrarConexion() {
        try {
            conexion.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void insertarPais(Country pais) {
        try {
            String sql = "INSERT INTO pais (nombre, capital, region, poblacion, latitud, longitud, codigo) VALUES (?,?,?,?,?,?,?)";
            PreparedStatement ps = conexion.prepareStatement(sql);
            ps.setString(1, pais.getNombrePais());
            ps.setString(2, pais.getCapitalPais());
            ps.setString(3, pais.getRegion());
            ps.setLong(4, pais.getPoblacion());
            ps.setDouble(5, pais.getLatitud());
            ps.setDouble(6, pais.getLongitud());
            ps.setInt(7, pais.getCodigoPais());
            ps.executeUpdate();
        } catch (SQLException ex) {
            Logger.getLogger(Gestor.class.getName()).log(Level.SEVERE, null, ex);
        }
    }

    public boolean consultarPais(int codigo) {
        try {
            String sql = "SELECT * FROM pais WHERE codigo = ?";
            PreparedStatement ps = conexion.prepareStatement(sql);
            ps.setInt(1, codigo);
            ResultSet rs = ps.executeQuery();
            if (rs.next()) {
                return true;
            }
        } catch (SQLException ex) {
            Logger.getLogger(Gestor.class.getName()).log(Level.SEVERE, null, ex);
        }
        return false;
    }

    public void updatePais(Country pais) {
        try {
            String sql = "UPDATE pais SET nombre = ?, capital = ?, region = ?, poblacion = ?, latitud = ?, longitud = ? WHERE codigo = ?";
            PreparedStatement ps = conexion.prepareStatement(sql);
            ps.setString(1, pais.getNombrePais());
            ps.setString(2, pais.getCapitalPais());
            ps.setString(3, pais.getRegion());
            ps.setLong(4, pais.getPoblacion());
            ps.setDouble(5, pais.getLatitud());
            ps.setDouble(6, pais.getLongitud());
            ps.setInt(7, pais.getCodigoPais());
            ps.executeUpdate();
        } catch (SQLException ex) {
            Logger.getLogger(Gestor.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
}
