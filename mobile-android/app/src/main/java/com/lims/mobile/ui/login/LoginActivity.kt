package com.lims.mobile.ui.login

import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.Toast
import androidx.activity.viewModels
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.lifecycleScope
import androidx.lifecycle.repeatOnLifecycle
import com.google.android.material.dialog.MaterialAlertDialogBuilder
import com.lims.mobile.databinding.ActivityLoginBinding
import com.lims.mobile.ui.home.HomeActivity
import com.lims.mobile.utils.NetworkUtils
import com.lims.mobile.utils.PreferencesManager
import kotlinx.coroutines.launch

class LoginActivity : AppCompatActivity() {

    private lateinit var binding: ActivityLoginBinding
    private val viewModel: LoginViewModel by viewModels {
        LoginViewModel.Factory(
            PreferencesManager(applicationContext),
            NetworkUtils.isEmulator()
        )
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityLoginBinding.inflate(layoutInflater)
        setContentView(binding.root)

        checkLoginStatus()
        setupViews()
        observeViewModel()
    }

    private fun checkLoginStatus() {
        lifecycleScope.launch {
            viewModel.isLoggedIn.collect { isLoggedIn ->
                if (isLoggedIn) {
                    navigateToHome()
                }
            }
        }
    }

    private fun setupViews() {
        // Login button
        binding.btnLogin.setOnClickListener {
            val username = binding.etUsername.text.toString().trim()
            val password = binding.etPassword.text.toString().trim()
            
            if (validateInput(username, password)) {
                viewModel.login(username, password)
            }
        }

        // Server config toggle
        binding.btnServerConfig.setOnClickListener {
            val isVisible = binding.layoutServerConfig.visibility == View.VISIBLE
            binding.layoutServerConfig.visibility = if (isVisible) View.GONE else View.VISIBLE
        }

        // Save server URL
        binding.btnSaveServer.setOnClickListener {
            val url = binding.etServerUrl.text.toString().trim()
            if (url.isNotEmpty()) {
                viewModel.setServerUrl(url)
                Toast.makeText(this, "服务器地址已保存", Toast.LENGTH_SHORT).show()
            }
        }

        // Test account chips
        binding.chipAdmin.setOnClickListener {
            binding.etUsername.setText("admin")
            binding.etPassword.setText("admin123")
        }
        binding.chipTeacher.setOnClickListener {
            binding.etUsername.setText("teacher")
            binding.etPassword.setText("teacher123")
        }
        binding.chipStudent.setOnClickListener {
            binding.etUsername.setText("student")
            binding.etPassword.setText("student123")
        }

        // Display current server URL
        lifecycleScope.launch {
            viewModel.currentServerUrl.collect { url ->
                binding.tvCurrentServer.text = "当前: $url"
            }
        }
    }

    private fun observeViewModel() {
        lifecycleScope.launch {
            repeatOnLifecycle(Lifecycle.State.STARTED) {
                launch {
                    viewModel.uiState.collect { state ->
                        when (state) {
                            is LoginUiState.Loading -> {
                                binding.progressBar.visibility = View.VISIBLE
                                binding.btnLogin.isEnabled = false
                                binding.btnLogin.text = getString(com.lims.mobile.R.string.btn_logging_in)
                            }
                            is LoginUiState.Success -> {
                                binding.progressBar.visibility = View.GONE
                                binding.btnLogin.isEnabled = true
                                binding.btnLogin.text = getString(com.lims.mobile.R.string.btn_login)
                                Toast.makeText(
                                    this@LoginActivity,
                                    "欢迎回来，${state.user.fullName ?: state.user.username}",
                                    Toast.LENGTH_SHORT
                                ).show()
                                navigateToHome()
                            }
                            is LoginUiState.Error -> {
                                binding.progressBar.visibility = View.GONE
                                binding.btnLogin.isEnabled = true
                                binding.btnLogin.text = getString(com.lims.mobile.R.string.btn_login)
                                showErrorDialog(state.message)
                            }
                            is LoginUiState.Idle -> {
                                binding.progressBar.visibility = View.GONE
                                binding.btnLogin.isEnabled = true
                                binding.btnLogin.text = getString(com.lims.mobile.R.string.btn_login)
                            }
                        }
                    }
                }
            }
        }
    }

    private fun validateInput(username: String, password: String): Boolean {
        var isValid = true
        
        if (username.isEmpty()) {
            binding.tilUsername.error = getString(com.lims.mobile.R.string.error_empty_username)
            isValid = false
        } else {
            binding.tilUsername.error = null
        }
        
        if (password.isEmpty()) {
            binding.tilPassword.error = getString(com.lims.mobile.R.string.error_empty_password)
            isValid = false
        } else {
            binding.tilPassword.error = null
        }
        
        return isValid
    }

    private fun showErrorDialog(message: String) {
        MaterialAlertDialogBuilder(this)
            .setTitle("登录失败")
            .setMessage(message)
            .setPositiveButton("确定", null)
            .setNegativeButton("配置服务器") { _, _ ->
                binding.layoutServerConfig.visibility = View.VISIBLE
            }
            .show()
    }

    private fun navigateToHome() {
        startActivity(Intent(this, HomeActivity::class.java))
        finish()
    }
}