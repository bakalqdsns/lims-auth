package com.lims.mobile.ui.home

import android.content.Intent
import android.os.Bundle
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.google.android.material.dialog.MaterialAlertDialogBuilder
import com.lims.mobile.R
import com.lims.mobile.databinding.ActivityHomeBinding
import com.lims.mobile.ui.login.LoginActivity
import com.lims.mobile.utils.PreferencesManager
import kotlinx.coroutines.launch

class HomeActivity : AppCompatActivity() {

    private lateinit var binding: ActivityHomeBinding
    private lateinit var prefs: PreferencesManager

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)
        
        prefs = PreferencesManager(applicationContext)
        
        setupToolbar()
        setupNavigation()
        loadUserInfo()
    }

    private fun setupToolbar() {
        setSupportActionBar(binding.toolbar)
        supportActionBar?.setDisplayHomeAsUpEnabled(true)
        supportActionBar?.setHomeAsUpIndicator(R.drawable.ic_menu)
        
        binding.toolbar.setNavigationOnClickListener {
            binding.drawerLayout.open()
        }
    }

    private fun setupNavigation() {
        binding.navView.setNavigationItemSelectedListener { menuItem ->
            when (menuItem.itemId) {
                R.id.nav_home -> {
                    binding.drawerLayout.close()
                    true
                }
                R.id.nav_profile -> {
                    Toast.makeText(this, "个人信息功能开发中", Toast.LENGTH_SHORT).show()
                    binding.drawerLayout.close()
                    true
                }
                R.id.nav_logout -> {
                    binding.drawerLayout.close()
                    showLogoutConfirmDialog()
                    true
                }
                else -> false
            }
        }
    }

    private fun loadUserInfo() {
        lifecycleScope.launch {
            prefs.user.collect { user ->
                user?.let {
                    // Update welcome card
                    binding.tvUsername.text = "用户名: ${it.username}"
                    binding.tvFullname.text = "姓名: ${it.fullName ?: it.username}"
                    
                    // Update role display
                    val roleText = when (it.role) {
                        "Admin" -> "管理员"
                        "Teacher" -> "教师"
                        "Student" -> "学生"
                        else -> it.role
                    }
                    binding.tvRole.text = "角色: $roleText"
                    binding.chipRole.text = roleText
                    
                    // Set role chip color
                    val roleColor = when (it.role) {
                        "Admin" -> R.color.role_admin
                        "Teacher" -> R.color.role_teacher
                        else -> R.color.role_student
                    }
                    binding.chipRole.setChipBackgroundColorResource(roleColor)
                    
                    // Update navigation header
                    val headerView = binding.navView.getHeaderView(0)
                    headerView.findViewById<TextView>(R.id.tv_nav_username)?.text = it.username
                    headerView.findViewById<TextView>(R.id.tv_nav_role)?.text = roleText
                }
            }
        }
    }

    private fun showLogoutConfirmDialog() {
        MaterialAlertDialogBuilder(this)
            .setTitle("退出登录")
            .setMessage("确定要退出登录吗？")
            .setPositiveButton("确定") { _, _ ->
                logout()
            }
            .setNegativeButton("取消", null)
            .show()
    }

    private fun logout() {
        lifecycleScope.launch {
            prefs.clear()
            Toast.makeText(this@HomeActivity, "已退出登录", Toast.LENGTH_SHORT).show()
            
            val intent = Intent(this@HomeActivity, LoginActivity::class.java)
            intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
            startActivity(intent)
        }
    }
}